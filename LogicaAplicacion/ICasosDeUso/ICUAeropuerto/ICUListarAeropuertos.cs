using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsAeropuerto;

namespace LogicaAplicacion.ICasosDeUso.ICUAeropuerto
{
    public interface ICUListarAeropuertos
    {
        // filtro: busca por código IATA, ciudad o país (null o vacío = todos)
        List<DTOAeropuerto> Ejecutar(string filtro);
    }
}
