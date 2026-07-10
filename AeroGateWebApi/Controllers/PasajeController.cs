using DTOs.DataTransferObjects.DTOsPasaje;
using DTOs.DataTransferObjects.DTOsUsuario;
using LogicaAplicacion.ICasosDeUso.ICUPasaje;
using LogicaNegocio.Excepciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeroGateWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PasajeController : ControllerBase
    {
        private readonly ICUComprarPasaje _cuComprar;
        private readonly ICUDevolverPasaje _cuDevolver;
        private readonly ICUMisPasajes _cuMisPasajes;
        private readonly ICUListarPasajerosPorVuelo _cuPasajerosPorVuelo;
        private readonly ICURankingDestinos _cuRanking;

        public PasajeController(
            ICUComprarPasaje cuComprar,
            ICUDevolverPasaje cuDevolver,
            ICUMisPasajes cuMisPasajes,
            ICUListarPasajerosPorVuelo cuPasajerosPorVuelo,
            ICURankingDestinos cuRanking)
        {
            _cuComprar = cuComprar;
            _cuDevolver = cuDevolver;
            _cuMisPasajes = cuMisPasajes;
            _cuPasajerosPorVuelo = cuPasajerosPorVuelo;
            _cuRanking = cuRanking;
        }

        /// <summary>
        /// RF06 – Compra de pasaje con descuento por categoría (solo Cliente, auditada).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "CLIENTE")]
        [ProducesResponseType(typeof(DTOPasaje), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Comprar([FromBody] DTOComprarPasaje dto)
        {
            try
            {
                // El cliente compra para sí mismo: su ID sale del JWT, nunca del body
                dto.ClienteId = ObtenerUsuarioId();
                DTOPasaje pasaje = _cuComprar.Ejecutar(dto);
                return StatusCode(StatusCodes.Status201Created, pasaje);
            }
            catch (UsuarioException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (VueloException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (PasajeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// RF07 – Monto a reintegrar por la devolución, para confirmar antes (solo Cliente).
        /// </summary>
        [HttpGet("{id:int}/monto-devolucion")]
        [Authorize(Roles = "CLIENTE")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetMontoDevolucion(int id)
        {
            try
            {
                return Ok(_cuDevolver.CalcularMonto(id, ObtenerUsuarioId()));
            }
            catch (PasajeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// RF07 – Devolución voluntaria con reintegro según categoría (solo Cliente, auditada).
        /// </summary>
        [HttpPost("{id:int}/devolucion")]
        [Authorize(Roles = "CLIENTE")]
        [ProducesResponseType(typeof(DTOPasaje), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Devolver(int id)
        {
            try
            {
                var dto = new DTODevolverPasaje { PasajeId = id, ClienteId = ObtenerUsuarioId() };
                return Ok(_cuDevolver.Ejecutar(dto));
            }
            catch (PasajeException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// RF08 – Mis pasajes, filtrables por mes y año (solo Cliente).
        /// </summary>
        [HttpGet("mis-pasajes")]
        [Authorize(Roles = "CLIENTE")]
        [ProducesResponseType(typeof(IEnumerable<DTOPasaje>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetMisPasajes([FromQuery] int? mes, [FromQuery] int? anio)
        {
            try
            {
                return Ok(_cuMisPasajes.Ejecutar(ObtenerUsuarioId(), mes, anio));
            }
            catch (PasajeException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// RF09 – Pasajeros con pasaje comprado de un vuelo (Administrador / Operador).
        /// </summary>
        [HttpGet("pasajeros-por-vuelo/{vueloId:int}")]
        [Authorize(Roles = "ADMINISTRADOR,OPERADOR")]
        [ProducesResponseType(typeof(IEnumerable<DTOCliente>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPasajerosPorVuelo(int vueloId)
        {
            try
            {
                return Ok(_cuPasajerosPorVuelo.Ejecutar(vueloId));
            }
            catch (VueloException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// RF10 – Ranking de destinos con más pasajes vendidos (todos los roles).
        /// </summary>
        [HttpGet("ranking-destinos")]
        [ProducesResponseType(typeof(IEnumerable<DTORankingDestino>), StatusCodes.Status200OK)]
        public IActionResult GetRankingDestinos()
        {
            return Ok(_cuRanking.Ejecutar());
        }

        private int ObtenerUsuarioId()
        {
            return int.Parse(User.FindFirst("Id").Value);
        }
    }
}
