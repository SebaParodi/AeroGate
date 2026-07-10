using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsUsuario;
using DTOs.Mappers;
using LogicaAplicacion.ICasosDeUso.ICUPasaje;
using LogicaNegocio.Entidades;
using LogicaNegocio.Excepciones;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUPasaje
{
    public class CUListarPasajerosPorVuelo : ICUListarPasajerosPorVuelo
    {
        private readonly IRepositorioPasaje _repoPasaje;
        private readonly IRepositorioVuelo _repoVuelo;

        public CUListarPasajerosPorVuelo(IRepositorioPasaje repoPasaje, IRepositorioVuelo repoVuelo)
        {
            _repoPasaje = repoPasaje;
            _repoVuelo = repoVuelo;
        }

        // RF09: pasajeros con pasaje comprado de un vuelo específico
        public List<DTOCliente> Ejecutar(int vueloId)
        {
            if (_repoVuelo.FindById(vueloId) == null)
                throw new VueloException($"No existe un vuelo con ID {vueloId}");

            List<Cliente> pasajeros = _repoPasaje.GetPasajerosPorVuelo(vueloId);
            return MapperUsuario.ToListDtoCliente(pasajeros);
        }
    }
}
