using System.ComponentModel.DataAnnotations.Schema;
using LogicaNegocio.Excepciones;
using LogicaNegocio.InterfacesDominio;

namespace LogicaNegocio.ValueObjects.VOUsuario
{
    [ComplexType]
    public class NombreCompletoVO : IValidable
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        public NombreCompletoVO() { }

        public NombreCompletoVO(string nombre, string apellido)
        {
            Nombre = nombre;
            Apellido = apellido;
        }

        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(Nombre)) throw new UsuarioException("El nombre es obligatorio");
            if (string.IsNullOrWhiteSpace(Apellido)) throw new UsuarioException("El apellido es obligatorio");
        }

        public override string ToString()
        {
            return $"{Nombre} {Apellido}";
        }
    }
}
