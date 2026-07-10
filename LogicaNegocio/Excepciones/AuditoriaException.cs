using System;

namespace LogicaNegocio.Excepciones
{
    public class AuditoriaException : Exception
    {
        public AuditoriaException(string mensaje) : base(mensaje) { }
    }
}
