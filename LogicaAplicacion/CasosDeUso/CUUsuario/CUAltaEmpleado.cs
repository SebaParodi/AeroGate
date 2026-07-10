using DTOs.DataTransferObjects.DTOsUsuario;
using LogicaAplicacion.ICasosDeUso.ICUUsuario;
using LogicaNegocio.Entidades;
using LogicaNegocio.Excepciones;
using LogicaNegocio.IRepositorios;
using LogicaNegocio.ValueObjects.VOUsuario;

namespace LogicaAplicacion.CasosDeUso.CUUsuario
{
    public class CUAltaEmpleado : ICUAltaEmpleado
    {
        private readonly IRepositorioUsuario _repoUsuario;

        public CUAltaEmpleado(IRepositorioUsuario repoUsuario)
        {
            _repoUsuario = repoUsuario;
        }

        public void Ejecutar(DTOAltaEmpleado dto)
        {
            if (_repoUsuario.FindByEmail(dto.Email) != null)
                throw new UsuarioException("Ya existe un usuario registrado con ese email");

            NombreCompletoVO nombre = new NombreCompletoVO(dto.Nombre, dto.Apellido);
            Empleado empleado = CrearSegunRol(dto, nombre);

            empleado.Validar();
            _repoUsuario.Add(empleado);
        }

        private Empleado CrearSegunRol(DTOAltaEmpleado dto, NombreCompletoVO nombre)
        {
            string rol = dto.Rol == null ? "" : dto.Rol.Trim().ToUpper();

            switch (rol)
            {
                case "ADMINISTRADOR":
                    return new Administrador(nombre, dto.Email, dto.Pass, dto.FechaIngreso);
                case "OPERADOR":
                    return new Operador(nombre, dto.Email, dto.Pass, dto.FechaIngreso);
                default:
                    throw new UsuarioException("El rol debe ser ADMINISTRADOR u OPERADOR");
            }
        }
    }
}
