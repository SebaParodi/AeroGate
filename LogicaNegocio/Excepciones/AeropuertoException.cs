using System;

namespace LogicaNegocio.Excepciones
{
    public class AeropuertoException : Exception
    {
        public AeropuertoException(string mensaje) : base(mensaje) { }
    }
}
