using System;

namespace LogicaNegocio.Excepciones
{
    public class VueloException : Exception
    {
        public VueloException(string mensaje) : base(mensaje) { }
    }
}
