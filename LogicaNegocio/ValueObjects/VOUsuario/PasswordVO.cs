using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using LogicaNegocio.Excepciones;
using LogicaNegocio.InterfacesDominio;

namespace LogicaNegocio.ValueObjects.VOUsuario
{
    [ComplexType]
    public class PasswordVO : IValidable
    {
        public string Valor { get; set; }

        public PasswordVO() { }

        public PasswordVO(string valor)
        {
            Valor = valor;
        }

        // Permite asignar directamente: PasswordVO pass = "Admin@123";
        public static implicit operator PasswordVO(string valor)
        {
            return new PasswordVO(valor);
        }

        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(Valor)) 
                throw new UsuarioException("La contraseña es obligatoria");
            if (Valor.Length < 8) 
                throw new UsuarioException("La contraseña debe tener al menos 8 caracteres");
            if (!Valor.Any(char.IsUpper)) 
                throw new UsuarioException("La contraseña debe tener al menos una mayúscula");
            if (!Valor.Any(char.IsDigit)) 
                throw new UsuarioException("La contraseña debe tener al menos un número");
            if (Valor.All(char.IsLetterOrDigit)) 
                throw new UsuarioException("La contraseña debe tener al menos un carácter especial");
        }

        // Comparación por valor, no por referencia
        public override bool Equals(object obj)
        {
            return obj is PasswordVO otro && Valor == otro.Valor;
        }

        public override int GetHashCode()   //<--- Explicar que es o como se conecta con el encifrado
        {
            return Valor is null ? 0 : Valor.GetHashCode();
        }

        public static bool operator ==(PasswordVO a, PasswordVO b) //<--- Como funciona, porque devuelve bool y que es el tag   operator
        {
            if (a is null) return b is null;
            return a.Equals(b);
        }

        public static bool operator !=(PasswordVO a, PasswordVO b)
        {
            return !(a == b);
        }
    }
}
