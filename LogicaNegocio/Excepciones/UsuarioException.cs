using System;

namespace LogicaNegocio.Excepciones
{
    public class UsuarioException : Exception
    {
        public UsuarioException(string mensaje) : base(mensaje) { }
    }
}
