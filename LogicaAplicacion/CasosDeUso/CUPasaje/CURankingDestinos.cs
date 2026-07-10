using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsPasaje;
using DTOs.Mappers;
using LogicaAplicacion.ICasosDeUso.ICUPasaje;
using LogicaNegocio.Consultas;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUPasaje
{
    public class CURankingDestinos : ICURankingDestinos
    {
        private readonly IRepositorioPasaje _repoPasaje;

        public CURankingDestinos(IRepositorioPasaje repoPasaje)
        {
            _repoPasaje = repoPasaje;
        }

        // RF10: destinos con más pasajes vendidos
        public List<DTORankingDestino> Ejecutar()
        {
            List<RankingDestino> ranking = _repoPasaje.GetRankingDestinos();
            return MapperPasaje.ToListDtoRankingDestino(ranking);
        }
    }
}
