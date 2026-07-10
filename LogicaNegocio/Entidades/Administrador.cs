using System;
using LogicaNegocio.ValueObjects.VOUsuario;

namespace LogicaNegocio.Entidades
{
    public class Administrador : Empleado
    {
        public Administrador() { }

        public Administrador(NombreCompletoVO nombreCompleto, string email, string pass, DateTime fechaIngreso)
            : base(nombreCompleto, email, pass, fechaIngreso)
        {
        }
    }
}
