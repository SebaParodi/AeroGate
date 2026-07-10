using System;
using LogicaNegocio.Excepciones;
using LogicaNegocio.ValueObjects.VOUsuario;

namespace LogicaNegocio.Entidades
{
    public abstract class Empleado : Usuario
    {
        public DateTime FechaIngreso { get; set; }

        protected Empleado() { }

        protected Empleado(NombreCompletoVO nombreCompleto, string email, string pass, DateTime fechaIngreso)
            : base(nombreCompleto, email, pass)
        {
            FechaIngreso = fechaIngreso;
        }

        public override void Validar()
        {
            base.Validar();
            if (FechaIngreso == default) throw new UsuarioException("La fecha de ingreso es obligatoria");
            if (FechaIngreso > DateTime.Now) throw new UsuarioException("La fecha de ingreso no puede ser futura");
        }
    }
}
