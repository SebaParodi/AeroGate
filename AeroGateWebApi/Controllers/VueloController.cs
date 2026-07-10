using DTOs.DataTransferObjects.DTOsVuelo;
using LogicaAplicacion.ICasosDeUso.ICUVuelo;
using LogicaNegocio.Excepciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeroGateWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VueloController : ControllerBase
    {
        private readonly ICUAltaVuelo _cuAlta;
        private readonly ICUModificarVuelo _cuModificar;
        private readonly ICUEliminarVuelo _cuEliminar;
        private readonly ICUListarVuelos _cuListar;
        private readonly ICUGetVueloPorId _cuGetPorId;
        private readonly ICUListarVuelosDisponibles _cuDisponibles;
        private readonly ICUCambiarEstadoVuelo _cuCambiarEstado;

        public VueloController(
            ICUAltaVuelo cuAlta,
            ICUModificarVuelo cuModificar,
            ICUEliminarVuelo cuEliminar,
            ICUListarVuelos cuListar,
            ICUGetVueloPorId cuGetPorId,
            ICUListarVuelosDisponibles cuDisponibles,
            ICUCambiarEstadoVuelo cuCambiarEstado)
        {
            _cuAlta = cuAlta;
            _cuModificar = cuModificar;
            _cuEliminar = cuEliminar;
            _cuListar = cuListar;
            _cuGetPorId = cuGetPorId;
            _cuDisponibles = cuDisponibles;
            _cuCambiarEstado = cuCambiarEstado;
        }

        /// <summary>
        /// RF04 – Listado de vuelos para gestión (Administrador / Operador).
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "ADMINISTRADOR,OPERADOR")]
        [ProducesResponseType(typeof(IEnumerable<DTOVuelo>), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            return Ok(_cuListar.Ejecutar());
        }

        /// <summary>
        /// RF06 – Vuelos disponibles para la venta (PROGRAMADO o DEMORADO, con asientos).
        /// </summary>
        [HttpGet("disponibles")]
        [ProducesResponseType(typeof(IEnumerable<DTOVuelo>), StatusCodes.Status200OK)]
        public IActionResult GetDisponibles()
        {
            return Ok(_cuDisponibles.Ejecutar());
        }

        /// <summary>
        /// Obtiene un vuelo por su ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DTOVuelo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            try
            {
                return Ok(_cuGetPorId.Ejecutar(id));
            }
            catch (VueloException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// RF04 – Alta de vuelo (solo Administrador, auditada).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADMINISTRADOR")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] DTOAltaVuelo dto)
        {
            try
            {
                dto.UsuarioId = ObtenerUsuarioId();
                _cuAlta.Ejecutar(dto);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (VueloException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// RF04 – Modificación de vuelo: solo en estado PROGRAMADO (solo Administrador, auditada).
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMINISTRADOR")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Update(int id, [FromBody] DTOModificarVuelo dto)
        {
            try
            {
                dto.Id = id;
                dto.UsuarioId = ObtenerUsuarioId();
                _cuModificar.Ejecutar(dto);
                return NoContent();
            }
            catch (VueloException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// RF04 – Baja lógica: solo vuelos sin pasajes vendidos (solo Administrador, auditada).
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMINISTRADOR")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Delete(int id)
        {
            try
            {
                _cuEliminar.Ejecutar(id, ObtenerUsuarioId());
                return NoContent();
            }
            catch (VueloException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// RF05 – Cambio de estado según la máquina de estados; CANCELADO dispara
        /// la devolución automática del 100 % (Administrador / Operador, auditada).
        /// </summary>
        [HttpPost("{id:int}/estado")]
        [Authorize(Roles = "ADMINISTRADOR,OPERADOR")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CambiarEstado(int id, [FromBody] DTOCambiarEstadoVuelo dto)
        {
            try
            {
                dto.VueloId = id;
                dto.UsuarioId = ObtenerUsuarioId();
                _cuCambiarEstado.Ejecutar(dto);
                return NoContent();
            }
            catch (VueloException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private int ObtenerUsuarioId()
        {
            return int.Parse(User.FindFirst("Id").Value);
        }
    }
}
