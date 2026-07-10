using DTOs.DataTransferObjects.DTOsAuditoria;
using LogicaAplicacion.ICasosDeUso.ICUAuditoria;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeroGateWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMINISTRADOR,OPERADOR")]
    public class AuditoriaController : ControllerBase
    {
        private readonly ICUListarAuditorias _cuListar;

        public AuditoriaController(ICUListarAuditorias cuListar)
        {
            _cuListar = cuListar;
        }

        /// <summary>
        /// RF11 – Auditoría de operaciones: el Administrador ve todas,
        /// el Operador únicamente las que realizó él.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DTOAuditoria>), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            // Con rol ADMINISTRADOR no se filtra (null); con OPERADOR se filtra por su ID
            int? usuarioId = User.IsInRole("ADMINISTRADOR")
                ? null
                : int.Parse(User.FindFirst("Id").Value);

            return Ok(_cuListar.Ejecutar(usuarioId));
        }
    }
}
