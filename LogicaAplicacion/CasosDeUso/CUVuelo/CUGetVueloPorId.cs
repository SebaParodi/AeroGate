using DTOs.DataTransferObjects.DTOsVuelo;
using DTOs.Mappers;
using LogicaAplicacion.ICasosDeUso.ICUVuelo;
using LogicaNegocio.Entidades;
using LogicaNegocio.Excepciones;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUVuelo
{
    public class CUGetVueloPorId : ICUGetVueloPorId
    {
        private readonly IRepositorioVuelo _repoVuelo;

        public CUGetVueloPorId(IRepositorioVuelo repoVuelo)
        {
            _repoVuelo = repoVuelo;
        }

        public DTOVuelo Ejecutar(int id)
        {
            Vuelo vuelo = _repoVuelo.FindById(id);
            if (vuelo == null || vuelo.Eliminado)
                throw new VueloException($"No existe un vuelo con ID {id}");

            return MapperVuelo.ToDtoVuelo(vuelo);
        }
    }
}
