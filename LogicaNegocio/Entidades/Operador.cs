using System;
using LogicaNegocio.ValueObjects.VOUsuario;

namespace LogicaNegocio.Entidades
{
    public class Operador : Empleado
    {
        public Operador() { }

        public Operador(NombreCompletoVO nombreCompleto, string email, string pass, DateTime fechaIngreso)
            : base(nombreCompleto, email, pass, fechaIngreso)
        {
        }
    }
}
