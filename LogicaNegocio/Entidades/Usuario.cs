using System.Text.RegularExpressions;
using LogicaNegocio.Excepciones;
using LogicaNegocio.InterfacesDominio;
using LogicaNegocio.ValueObjects.VOUsuario;

namespace LogicaNegocio.Entidades
{
    // Raíz de la jerarquía TPH: Usuario → Cliente | Empleado → Administrador | Operador
    public abstract class Usuario : IValidable
    {
        public int Id { get; set; }
        public NombreCompletoVO NombreCompleto { get; set; }
        public string Email { get; set; }
        public PasswordVO Pass { get; set; }

        protected Usuario() { }

        protected Usuario(NombreCompletoVO nombreCompleto, string email, string pass)
        {
            NombreCompleto = nombreCompleto;
            Email = email;
            Pass = pass;
        }

        public virtual void Validar()
        {
            if (NombreCompleto is null) throw new UsuarioException("El nombre completo es obligatorio");
            NombreCompleto.Validar();

            if (Pass is null) throw new UsuarioException("La contraseña es obligatoria");
            Pass.Validar();

            ValidarEmail();
        }

        private void ValidarEmail()
        {
            if (string.IsNullOrWhiteSpace(Email)) throw new UsuarioException("El email es obligatorio");
            if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) throw new UsuarioException("El formato del email no es válido");
        }
    }
}
