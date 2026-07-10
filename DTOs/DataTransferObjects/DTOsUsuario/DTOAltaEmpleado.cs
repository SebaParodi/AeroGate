using System;

namespace DTOs.DataTransferObjects.DTOsUsuario
{
    // RF02: alta de usuarios internos (Rol: "ADMINISTRADOR" u "OPERADOR")
    public class DTOAltaEmpleado
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public string Rol { get; set; }
        public DateTime FechaIngreso { get; set; }
    }
}
