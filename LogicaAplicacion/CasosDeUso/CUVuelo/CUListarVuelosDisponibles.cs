using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsVuelo;
using DTOs.Mappers;
using LogicaAplicacion.ICasosDeUso.ICUVuelo;
using LogicaNegocio.Entidades;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUVuelo
{
    public class CUListarVuelosDisponibles : ICUListarVuelosDisponibles
    {
        private readonly IRepositorioVuelo _repoVuelo;

        public CUListarVuelosDisponibles(IRepositorioVuelo repoVuelo)
        {
            _repoVuelo = repoVuelo;
        }

        // RF06: lo que ve el cliente para comprar
        public List<DTOVuelo> Ejecutar()
        {
            List<Vuelo> vuelos = _repoVuelo.GetDisponiblesParaVenta();
            return MapperVuelo.ToListDtoVuelo(vuelos);
        }
    }
}
