using System.Collections.Generic;
using LogicaAplicacion.ICasosDeUso.ICUAuditoria;
using LogicaAplicacion.ICasosDeUso.ICUVuelo;
using LogicaNegocio.Entidades;
using LogicaNegocio.Enumerados;
using LogicaNegocio.Excepciones;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUVuelo
{
    public class CUEliminarVuelo : ICUEliminarVuelo
    {
        private readonly IRepositorioVuelo _repoVuelo;
        private readonly IRepositorioPasaje _repoPasaje;
        private readonly ICUGenerarAuditoria _cuAuditoria;

        public CUEliminarVuelo(IRepositorioVuelo repoVuelo, IRepositorioPasaje repoPasaje,
                               ICUGenerarAuditoria cuAuditoria)
        {
            _repoVuelo = repoVuelo;
            _repoPasaje = repoPasaje;
            _cuAuditoria = cuAuditoria;
        }

        public void Ejecutar(int vueloId, int usuarioId)
        {
            Vuelo vuelo = _repoVuelo.FindById(vueloId);
            if (vuelo == null || vuelo.Eliminado)
                throw new VueloException($"No existe un vuelo con ID {vueloId}");

            // RF04: un vuelo con pasajes vendidos no puede eliminarse, solo cancelarse (RF05)
            List<Pasaje> pasajesActivos = _repoPasaje.GetActivosByVuelo(vueloId);
            if (pasajesActivos.Count > 0)
                throw new VueloException($"El vuelo {vuelo.Codigo} tiene {pasajesActivos.Count} pasaje(s) vendido(s): solo puede cancelarse");

            // Baja lógica
            vuelo.Eliminado = true;
            _repoVuelo.Update(vuelo);

            _cuAuditoria.Ejecutar(usuarioId, TipoOperacion.BAJA_VUELO,
                $"Baja lógica de vuelo {vuelo.Codigo}", vueloId: vuelo.Id);
        }
    }
}
