using DTOs.DataTransferObjects.DTOsUsuario;
using LogicaAplicacion.ICasosDeUso.ICUUsuario;
using LogicaNegocio.Excepciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeroGateWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly ICURegistrarCliente _cuRegistrarCliente;
        private readonly ICUAltaEmpleado _cuAltaEmpleado;
        private readonly ICUListarUsuarios _cuListarUsuarios;
        private readonly ICUListarClientes _cuListarClientes;

        public UsuarioController(
            ICURegistrarCliente cuRegistrarCliente,
            ICUAltaEmpleado cuAltaEmpleado,
            ICUListarUsuarios cuListarUsuarios,
            ICUListarClientes cuListarClientes)
        {
            _cuRegistrarCliente = cuRegistrarCliente;
            _cuAltaEmpleado = cuAltaEmpleado;
            _cuListarUsuarios = cuListarUsuarios;
            _cuListarClientes = cuListarClientes;
        }

        /// <summary>
        /// RF01 – Registro público de cliente (categoría inicial Estándar).
        /// </summary>
        [HttpPost("registro")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Registro([FromBody] DTORegistrarCliente dto)
        {
            try
            {
                _cuRegistrarCliente.Ejecutar(dto);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (UsuarioException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// RF02 – Alta de usuarios internos: Administradores y Operadores (solo Administrador).
        /// </summary>
        [HttpPost("empleados")]
        [Authorize(Roles = "ADMINISTRADOR")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AltaEmpleado([FromBody] DTOAltaEmpleado dto)
        {
            try
            {
                _cuAltaEmpleado.Ejecutar(dto);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (UsuarioException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Listado de todos los usuarios del sistema (solo Administrador).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "ADMINISTRADOR")]
        [ProducesResponseType(typeof(IEnumerable<DTOUsuario>), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            return Ok(_cuListarUsuarios.Ejecutar());
        }

        /// <summary>
        /// Listado de clientes (Administrador / Operador).
        /// </summary>
        [HttpGet("clientes")]
        [Authorize(Roles = "ADMINISTRADOR,OPERADOR")]
        [ProducesResponseType(typeof(IEnumerable<DTOCliente>), StatusCodes.Status200OK)]
        public IActionResult GetClientes()
        {
            return Ok(_cuListarClientes.Ejecutar());
        }
    }
}
