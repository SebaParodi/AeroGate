using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsVuelo;
using DTOs.Mappers;
using LogicaAplicacion.ICasosDeUso.ICUVuelo;
using LogicaNegocio.Entidades;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUVuelo
{
    public class CUListarVuelos : ICUListarVuelos
    {
        private readonly IRepositorioVuelo _repoVuelo;

        public CUListarVuelos(IRepositorioVuelo repoVuelo)
        {
            _repoVuelo = repoVuelo;
        }

        public List<DTOVuelo> Ejecutar()
        {
            List<Vuelo> vuelos = _repoVuelo.GetAll();
            return MapperVuelo.ToListDtoVuelo(vuelos);
        }
    }
}
