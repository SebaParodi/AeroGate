using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsPasaje;

namespace LogicaAplicacion.ICasosDeUso.ICUPasaje
{
    public interface ICURankingDestinos
    {
        List<DTORankingDestino> Ejecutar();
    }
}
