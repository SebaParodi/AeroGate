using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsPasaje;

namespace LogicaAplicacion.ICasosDeUso.ICUPasaje
{
    public interface ICUMisPasajes
    {
        // RF08: mes y año opcionales (ambos o ninguno)
        List<DTOPasaje> Ejecutar(int clienteId, int? mes, int? anio);
    }
}
