using LogicaAplicacion.ICasosDeUso.ICUUsuario;
using LogicaNegocio.Entidades;
using LogicaNegocio.Excepciones;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUUsuario
{
    public class CULogin : ICULogin
    {
        private readonly IRepositorioUsuario _repoUsuario;

        public CULogin(IRepositorioUsuario repoUsuario)
        {
            _repoUsuario = repoUsuario;
        }

        public Usuario Ejecutar(string email, string pass)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
                throw new UsuarioException("Email y contraseña son requeridos");

            Usuario usuario = _repoUsuario.FindByEmail(email);

            if (usuario == null || usuario.Pass != pass)
                throw new UsuarioException("Credenciales inválidas");

            return usuario;
        }
    }
}
