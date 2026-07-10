using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsVuelo;

namespace LogicaAplicacion.ICasosDeUso.ICUVuelo
{
    public interface ICUListarVuelosDisponibles
    {
        // Vuelos en venta para el cliente: PROGRAMADO o DEMORADO, con asientos
        List<DTOVuelo> Ejecutar();
    }
}
