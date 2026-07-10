using DTOs.DataTransferObjects.DTOsUsuario;
using LogicaAplicacion.ICasosDeUso.ICUUsuario;
using LogicaNegocio.Entidades;
using LogicaNegocio.Excepciones;
using LogicaNegocio.IRepositorios;
using LogicaNegocio.ValueObjects.VOUsuario;

namespace LogicaAplicacion.CasosDeUso.CUUsuario
{
    public class CURegistrarCliente : ICURegistrarCliente
    {
        private readonly IRepositorioUsuario _repoUsuario;

        public CURegistrarCliente(IRepositorioUsuario repoUsuario)
        {
            _repoUsuario = repoUsuario;
        }

        public void Ejecutar(DTORegistrarCliente dto)
        {
            // Unicidad de email (la validación de formato la hace la entidad)
            if (_repoUsuario.FindByEmail(dto.Email) != null)
                throw new UsuarioException("Ya existe un usuario registrado con ese email");

            NombreCompletoVO nombre = new NombreCompletoVO(dto.Nombre, dto.Apellido);
            Cliente cliente = new Cliente(nombre, dto.Email, dto.Pass, dto.Documento, dto.Direccion);

            cliente.Validar();
            _repoUsuario.Add(cliente);
        }
    }
}
