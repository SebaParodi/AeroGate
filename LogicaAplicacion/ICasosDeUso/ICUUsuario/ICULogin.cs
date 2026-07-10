using LogicaNegocio.Entidades;

namespace LogicaAplicacion.ICasosDeUso.ICUUsuario
{
    public interface ICULogin
    {
        // Devuelve la entidad: la API arma el JWT con el rol derivado del tipo concreto
        Usuario Ejecutar(string email, string pass);
    }
}
