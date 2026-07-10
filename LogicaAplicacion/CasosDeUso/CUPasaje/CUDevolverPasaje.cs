using DTOs.DataTransferObjects.DTOsPasaje;
using DTOs.Mappers;
using LogicaAplicacion.ICasosDeUso.ICUAuditoria;
using LogicaAplicacion.ICasosDeUso.ICUPasaje;
using LogicaNegocio.Entidades;
using LogicaNegocio.Enumerados;
using LogicaNegocio.Excepciones;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUPasaje
{
    public class CUDevolverPasaje : ICUDevolverPasaje
    {
        private readonly IRepositorioPasaje _repoPasaje;
        private readonly IRepositorioVuelo _repoVuelo;
        private readonly ICUGenerarAuditoria _cuAuditoria;

        public CUDevolverPasaje(IRepositorioPasaje repoPasaje, IRepositorioVuelo repoVuelo,
                                ICUGenerarAuditoria cuAuditoria)
        {
            _repoPasaje = repoPasaje;
            _repoVuelo = repoVuelo;
            _cuAuditoria = cuAuditoria;
        }

        // RF07: monto a reintegrar según la categoría ACTUAL del cliente, sin efectos
        public decimal CalcularMonto(int pasajeId, int clienteId)
        {
            Pasaje pasaje = ObtenerPasajeDevolvible(pasajeId, clienteId);
            return pasaje.Cliente.CalcularReintegro(pasaje.PrecioPagado);
        }

        public DTOPasaje Ejecutar(DTODevolverPasaje dto)
        {
            Pasaje pasaje = ObtenerPasajeDevolvible(dto.PasajeId, dto.ClienteId);

            // 1. Devolver: fija estado, fecha y monto (foto en el pasaje)
            decimal monto = pasaje.Cliente.CalcularReintegro(pasaje.PrecioPagado);
            pasaje.Devolver(monto);

            // 2. Reponer el asiento al vuelo
            pasaje.Vuelo.ReponerAsiento();

            _repoPasaje.Update(pasaje);
            _repoVuelo.Update(pasaje.Vuelo);

            // 3. Auditoría automática (RF07)
            _cuAuditoria.Ejecutar(dto.ClienteId, TipoOperacion.DEVOLUCION_PASAJE,
                $"Devolución de pasaje del vuelo {pasaje.Vuelo.Codigo}, reintegro ${monto}",
                vueloId: pasaje.VueloId, pasajeId: pasaje.Id);

            return MapperPasaje.ToDtoPasaje(pasaje);
        }

        // Reglas comunes: existencia, pertenencia al cliente y ventana de devolución
        private Pasaje ObtenerPasajeDevolvible(int pasajeId, int clienteId)
        {
            Pasaje pasaje = _repoPasaje.FindById(pasajeId);
            if (pasaje == null)
                throw new PasajeException($"No existe un pasaje con ID {pasajeId}");
            if (pasaje.ClienteId != clienteId)
                throw new PasajeException("El pasaje no pertenece al cliente");
            if (pasaje.Cliente == null || pasaje.Vuelo == null)
                throw new PasajeException("El pasaje no tiene cargados el cliente y el vuelo");
            if (!pasaje.EsDevolvible())
                throw new PasajeException("El pasaje ya no admite devolución (devuelto o vuelo en embarque/finalizado)");

            return pasaje;
        }
    }
}
