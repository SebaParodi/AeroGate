using AeroGateWebApp.Auxiliar;
using AeroGateWebApp.Enums;
using AeroGateWebApp.Filter;
using AeroGateWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AeroGateWebApp.Controllers
{
    public class AeropuertoController : Controller
    {
        private readonly string baseUrl;

        public AeropuertoController(IConfiguration config)
            => baseUrl = config["ApiSettings:BaseUrl"];

        private string Token => HttpContext.Session.GetString("token");

        // ── RF12: Listado con búsqueda ─────────────────────────────────────────

        [AdminYOperadorFilter]
        public IActionResult Index(string filtro)
        {
            string q = string.IsNullOrWhiteSpace(filtro) ? "" : $"?filtro={Uri.EscapeDataString(filtro)}";
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "aeropuerto" + q, VerbosHttp.GET, null, Token);

            var aeropuertos = resp.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<VMAeropuerto>>(ClienteHttpAuxiliar.ObtenerBody(resp))
                : new List<VMAeropuerto>();

            ViewBag.Filtro = filtro;
            return View(aeropuertos);
        }

        // ── RF12: Alta ─────────────────────────────────────────────────────────

        [AdminYOperadorFilter]
        public IActionResult Create() => View(new VMCrearAeropuerto());

        [HttpPost]
        [AdminYOperadorFilter]
        public IActionResult Create(VMCrearAeropuerto vm)
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "aeropuerto", VerbosHttp.POST, vm, Token);

            if (resp.IsSuccessStatusCode)
            {
                TempData["Exito"] = $"Aeropuerto {vm.CodigoIATA} creado correctamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Error = ManejadorErroresApi.ObtenerMensajeError(resp);
            return View(vm);
        }

        // ── RF12: Modificación (el código IATA es inmutable) ──────────────────

        [AdminYOperadorFilter]
        public IActionResult Edit(int id)
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + $"aeropuerto/{id}", VerbosHttp.GET, null, Token);

            if (!resp.IsSuccessStatusCode) return NotFound();

            var aeropuerto = JsonConvert.DeserializeObject<VMAeropuerto>(
                ClienteHttpAuxiliar.ObtenerBody(resp));
            return View(aeropuerto);
        }

        [HttpPost]
        [AdminYOperadorFilter]
        public IActionResult Edit(VMAeropuerto vm)
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + $"aeropuerto/{vm.Id}", VerbosHttp.PUT, vm, Token);

            if (resp.IsSuccessStatusCode)
            {
                TempData["Exito"] = $"Aeropuerto {vm.CodigoIATA} modificado correctamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Error = ManejadorErroresApi.ObtenerMensajeError(resp);
            return View(vm);
        }
    }
}
