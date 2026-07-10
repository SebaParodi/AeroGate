using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsUsuario;

namespace LogicaAplicacion.ICasosDeUso.ICUUsuario
{
    public interface ICUListarClientes
    {
        List<DTOCliente> Ejecutar();
    }
}
