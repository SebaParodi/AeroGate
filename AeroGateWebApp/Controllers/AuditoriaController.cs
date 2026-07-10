using AeroGateWebApp.Auxiliar;
using AeroGateWebApp.Enums;
using AeroGateWebApp.Filter;
using AeroGateWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AeroGateWebApp.Controllers
{
    public class AuditoriaController : Controller
    {
        private readonly string baseUrl;

        public AuditoriaController(IConfiguration config)
            => baseUrl = config["ApiSettings:BaseUrl"];

        private string Token => HttpContext.Session.GetString("token");

        // ── RF11: Auditoría — la API ya filtra por rol (admin: todas, operador: las suyas).
        //    Acá se agregan los filtros de la letra: tipo de operación y rango de fechas.

        [AdminYOperadorFilter]
        public IActionResult Index(string operacion, DateTime? desde, DateTime? hasta)
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "auditoria", VerbosHttp.GET, null, Token);

            var auditorias = resp.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<VMAuditoria>>(ClienteHttpAuxiliar.ObtenerBody(resp))
                : new List<VMAuditoria>();

            // Dropdown con los tipos de operación presentes en los datos
            ViewBag.Operaciones = auditorias.Select(a => a.Operacion).Distinct().OrderBy(o => o).ToList();

            if (!string.IsNullOrEmpty(operacion))
                auditorias = auditorias.Where(a => a.Operacion == operacion).ToList();
            if (desde.HasValue)
                auditorias = auditorias.Where(a => a.FechaHora >= desde.Value).ToList();
            if (hasta.HasValue)
                auditorias = auditorias.Where(a => a.FechaHora < hasta.Value.AddDays(1)).ToList();

            ViewBag.Operacion = operacion;
            ViewBag.Desde = desde;
            ViewBag.Hasta = hasta;
            return View(auditorias);
        }
    }
}
