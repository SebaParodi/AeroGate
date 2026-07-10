using DTOs.DataTransferObjects.DTOsVuelo;
using LogicaAplicacion.ICasosDeUso.ICUAuditoria;
using LogicaAplicacion.ICasosDeUso.ICUVuelo;
using LogicaNegocio.Entidades;
using LogicaNegocio.Enumerados;
using LogicaNegocio.Excepciones;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUVuelo
{
    public class CUAltaVuelo : ICUAltaVuelo
    {
        private readonly IRepositorioVuelo _repoVuelo;
        private readonly IRepositorioAeropuerto _repoAeropuerto;
        private readonly ICUGenerarAuditoria _cuAuditoria;

        public CUAltaVuelo(IRepositorioVuelo repoVuelo, IRepositorioAeropuerto repoAeropuerto,
                           ICUGenerarAuditoria cuAuditoria)
        {
            _repoVuelo = repoVuelo;
            _repoAeropuerto = repoAeropuerto;
            _cuAuditoria = cuAuditoria;
        }

        public void Ejecutar(DTOAltaVuelo dto)
        {
            // 1. Crear la entidad y validar (formato de código, fechas, origen ≠ destino, precio, capacidad)
            Vuelo vuelo = new Vuelo(dto.Codigo, dto.AeropuertoOrigenId, dto.AeropuertoDestinoId,
                                    dto.FechaSalida, dto.FechaLlegadaEstimada, dto.PrecioBase, dto.CapacidadAsientos);
            vuelo.Validar();

            // 2. Reglas que necesitan repositorio: código único y aeropuertos existentes
            if (_repoVuelo.FindByCodigo(dto.Codigo) != null)
                throw new VueloException($"Ya existe un vuelo con el código {dto.Codigo}");
            ValidarAeropuertoExiste(dto.AeropuertoOrigenId, "origen");
            ValidarAeropuertoExiste(dto.AeropuertoDestinoId, "destino");

            // 3. Persistir + auditoría automática (RF04)
            _repoVuelo.Add(vuelo);
            _cuAuditoria.Ejecutar(dto.UsuarioId, TipoOperacion.ALTA_VUELO,
                $"Alta de vuelo {vuelo.Codigo}", vueloId: vuelo.Id);
        }

        private void ValidarAeropuertoExiste(int aeropuertoId, string rolEnRuta)
        {
            if (_repoAeropuerto.FindById(aeropuertoId) == null)
                throw new VueloException($"El aeropuerto de {rolEnRuta} (ID {aeropuertoId}) no existe");
        }
    }
}
