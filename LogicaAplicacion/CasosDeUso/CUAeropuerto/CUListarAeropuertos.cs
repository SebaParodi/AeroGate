using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsAeropuerto;
using DTOs.Mappers;
using LogicaAplicacion.ICasosDeUso.ICUAeropuerto;
using LogicaNegocio.Entidades;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUAeropuerto
{
    public class CUListarAeropuertos : ICUListarAeropuertos
    {
        private readonly IRepositorioAeropuerto _repoAeropuerto;

        public CUListarAeropuertos(IRepositorioAeropuerto repoAeropuerto)
        {
            _repoAeropuerto = repoAeropuerto;
        }

        public List<DTOAeropuerto> Ejecutar(string filtro)
        {
            List<Aeropuerto> aeropuertos = _repoAeropuerto.GetAll(filtro);
            return MapperAeropuerto.ToListDtoAeropuerto(aeropuertos);
        }
    }
}
