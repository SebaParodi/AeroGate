using DTOs.DataTransferObjects.DTOsAeropuerto;
using LogicaAplicacion.ICasosDeUso.ICUAeropuerto;
using LogicaNegocio.Excepciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeroGateWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AeropuertoController : ControllerBase
    {
        private readonly ICUAltaAeropuerto _cuAlta;
        private readonly ICUModificarAeropuerto _cuModificar;
        private readonly ICUListarAeropuertos _cuListar;
        private readonly ICUGetAeropuertoPorId _cuGetPorId;

        public AeropuertoController(
            ICUAltaAeropuerto cuAlta,
            ICUModificarAeropuerto cuModificar,
            ICUListarAeropuertos cuListar,
            ICUGetAeropuertoPorId cuGetPorId)
        {
            _cuAlta = cuAlta;
            _cuModificar = cuModificar;
            _cuListar = cuListar;
            _cuGetPorId = cuGetPorId;
        }

        /// <summary>
        /// RF12 – Listado del catálogo con búsqueda por código IATA, ciudad o país.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DTOAeropuerto>), StatusCodes.Status200OK)]
        public IActionResult GetAll([FromQuery] string filtro = null)
        {
            return Ok(_cuListar.Ejecutar(filtro));
        }

        /// <summary>
        /// Obtiene un aeropuerto por su ID (para el formulario de edición).
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DTOAeropuerto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            try
            {
                return Ok(_cuGetPorId.Ejecutar(id));
            }
            catch (AeropuertoException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// RF12 – Alta de aeropuerto con código IATA único (Administrador / Operador, auditada).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADMINISTRADOR,OPERADOR")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] DTOAltaAeropuerto dto)
        {
            try
            {
                // Quién realiza la operación sale del JWT, nunca del body
                dto.UsuarioId = ObtenerUsuarioId();
                _cuAlta.Ejecutar(dto);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (AeropuertoException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// RF12 – Modificación de datos descriptivos; el código IATA es inmutable (auditada).
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMINISTRADOR,OPERADOR")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Update(int id, [FromBody] DTOModificarAeropuerto dto)
        {
            try
            {
                dto.Id = id;
                dto.UsuarioId = ObtenerUsuarioId();
                _cuModificar.Ejecutar(dto);
                return NoContent();
            }
            catch (AeropuertoException ex)
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
