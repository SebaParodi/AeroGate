using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsUsuario;

namespace LogicaAplicacion.ICasosDeUso.ICUUsuario
{
    public interface ICUListarUsuarios
    {
        List<DTOUsuario> Ejecutar();
    }
}
