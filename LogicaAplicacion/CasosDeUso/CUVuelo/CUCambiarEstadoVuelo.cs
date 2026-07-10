using System;
using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsVuelo;
using LogicaAplicacion.ICasosDeUso.ICUAuditoria;
using LogicaAplicacion.ICasosDeUso.ICUVuelo;
using LogicaNegocio.Entidades;
using LogicaNegocio.Enumerados;
using LogicaNegocio.Excepciones;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUVuelo
{
    public class CUCambiarEstadoVuelo : ICUCambiarEstadoVuelo
    {
        private readonly IRepositorioVuelo _repoVuelo;
        private readonly IRepositorioPasaje _repoPasaje;
        private readonly ICUGenerarAuditoria _cuAuditoria;

        public CUCambiarEstadoVuelo(IRepositorioVuelo repoVuelo, IRepositorioPasaje repoPasaje,
                                    ICUGenerarAuditoria cuAuditoria)
        {
            _repoVuelo = repoVuelo;
            _repoPasaje = repoPasaje;
            _cuAuditoria = cuAuditoria;
        }

        public void Ejecutar(DTOCambiarEstadoVuelo dto)
        {
            Vuelo vuelo = _repoVuelo.FindById(dto.VueloId);
            if (vuelo == null || vuelo.Eliminado)
                throw new VueloException($"No existe un vuelo con ID {dto.VueloId}");

            if (!Enum.TryParse(dto.NuevoEstado, true, out EstadoVuelo nuevoEstado))
                throw new VueloException($"'{dto.NuevoEstado}' no es un estado de vuelo válido");

            // 1. La entidad valida la transición (máquina de estados, RF05)
            vuelo.CambiarEstado(nuevoEstado);
            _repoVuelo.Update(vuelo);

            // 2. Cancelación: devolución automática del 100 % a todos los pasajes comprados
            if (nuevoEstado == EstadoVuelo.CANCELADO)
            {
                DevolverPasajesAutomaticamente(vuelo, dto.UsuarioId);
                _cuAuditoria.Ejecutar(dto.UsuarioId, TipoOperacion.CANCELACION_VUELO,
                    $"Cancelación de vuelo {vuelo.Codigo}", vueloId: vuelo.Id);
            }
            else
            {
                _cuAuditoria.Ejecutar(dto.UsuarioId, TipoOperacion.CAMBIO_ESTADO,
                    $"Vuelo {vuelo.Codigo} pasa a {nuevoEstado}", vueloId: vuelo.Id);
            }
        }

        private void DevolverPasajesAutomaticamente(Vuelo vuelo, int usuarioId)
        {
            List<Pasaje> pasajes = _repoPasaje.GetActivosByVuelo(vuelo.Id);

            foreach (Pasaje pasaje in pasajes)
            {
                // 100 % sin importar la categoría del cliente (LETRA 3.3)
                pasaje.Devolver(pasaje.PrecioPagado);
                _repoPasaje.Update(pasaje);

                _cuAuditoria.Ejecutar(usuarioId, TipoOperacion.DEVOLUCION_AUTOMATICA,
                    $"Devolución automática por cancelación del vuelo {vuelo.Codigo}",
                    vueloId: vuelo.Id, pasajeId: pasaje.Id);
            }
        }
    }
}
