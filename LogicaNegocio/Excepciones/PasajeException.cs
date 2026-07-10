using System;

namespace LogicaNegocio.Excepciones
{
    public class PasajeException : Exception
    {
        public PasajeException(string mensaje) : base(mensaje) { }
    }
}
