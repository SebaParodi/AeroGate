using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsVuelo;

namespace LogicaAplicacion.ICasosDeUso.ICUVuelo
{
    public interface ICUListarVuelos
    {
        List<DTOVuelo> Ejecutar();
    }
}
