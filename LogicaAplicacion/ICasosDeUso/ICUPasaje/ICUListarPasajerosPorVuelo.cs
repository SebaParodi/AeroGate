using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsUsuario;

namespace LogicaAplicacion.ICasosDeUso.ICUPasaje
{
    public interface ICUListarPasajerosPorVuelo
    {
        List<DTOCliente> Ejecutar(int vueloId);
    }
}
