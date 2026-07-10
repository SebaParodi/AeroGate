using DTOs.DataTransferObjects.DTOsVuelo;
using LogicaAplicacion.ICasosDeUso.ICUAuditoria;
using LogicaAplicacion.ICasosDeUso.ICUVuelo;
using LogicaNegocio.Entidades;
using LogicaNegocio.Enumerados;
using LogicaNegocio.Excepciones;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUVuelo
{
    public class CUModificarVuelo : ICUModificarVuelo
    {
        private readonly IRepositorioVuelo _repoVuelo;
        private readonly IRepositorioAeropuerto _repoAeropuerto;
        private readonly ICUGenerarAuditoria _cuAuditoria;

        public CUModificarVuelo(IRepositorioVuelo repoVuelo, IRepositorioAeropuerto repoAeropuerto,
                                ICUGenerarAuditoria cuAuditoria)
        {
            _repoVuelo = repoVuelo;
            _repoAeropuerto = repoAeropuerto;
            _cuAuditoria = cuAuditoria;
        }

        public void Ejecutar(DTOModificarVuelo dto)
        {
            Vuelo vuelo = _repoVuelo.FindById(dto.Id);
            if (vuelo == null || vuelo.Eliminado)
                throw new VueloException($"No existe un vuelo con ID {dto.Id}");

            // RF04: un vuelo solo admite modificaciones mientras está PROGRAMADO
            if (!vuelo.PuedeModificarse())
                throw new VueloException($"El vuelo {vuelo.Codigo} está {vuelo.Estado} y ya no admite modificaciones");

            ValidarAeropuertoExiste(dto.AeropuertoOrigenId, "origen");
            ValidarAeropuertoExiste(dto.AeropuertoDestinoId, "destino");

            // No se puede reducir la capacidad por debajo de los asientos ya vendidos
            int asientosVendidos = vuelo.CapacidadAsientos - vuelo.AsientosDisponibles;
            if (dto.CapacidadAsientos < asientosVendidos)
                throw new VueloException($"La capacidad no puede ser menor a los {asientosVendidos} asientos ya vendidos");

            // El código es inmutable; se actualiza el resto
            vuelo.AeropuertoOrigenId = dto.AeropuertoOrigenId;
            vuelo.AeropuertoDestinoId = dto.AeropuertoDestinoId;
            vuelo.FechaSalida = dto.FechaSalida;
            vuelo.FechaLlegadaEstimada = dto.FechaLlegadaEstimada;
            vuelo.PrecioBase = dto.PrecioBase;
            vuelo.AsientosDisponibles = dto.CapacidadAsientos - asientosVendidos;
            vuelo.CapacidadAsientos = dto.CapacidadAsientos;

            vuelo.Validar();
            _repoVuelo.Update(vuelo);

            _cuAuditoria.Ejecutar(dto.UsuarioId, TipoOperacion.MODIFICACION_VUELO,
                $"Modificación de vuelo {vuelo.Codigo}", vueloId: vuelo.Id);
        }

        private void ValidarAeropuertoExiste(int aeropuertoId, string rolEnRuta)
        {
            if (_repoAeropuerto.FindById(aeropuertoId) == null)
                throw new VueloException($"El aeropuerto de {rolEnRuta} (ID {aeropuertoId}) no existe");
        }
    }
}
