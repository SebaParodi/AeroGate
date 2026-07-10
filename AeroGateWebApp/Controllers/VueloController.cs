using AeroGateWebApp.Auxiliar;
using AeroGateWebApp.Enums;
using AeroGateWebApp.Filter;
using AeroGateWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace AeroGateWebApp.Controllers
{
    public class VueloController : Controller
    {
        private readonly string baseUrl;

        public VueloController(IConfiguration config)
            => baseUrl = config["ApiSettings:BaseUrl"];

        private string Token => HttpContext.Session.GetString("token");

        // ── RF04: Listado para gestión ─────────────────────────────────────────

        [AdminYOperadorFilter]
        public IActionResult Index()
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "vuelo", VerbosHttp.GET, null, Token);

            var vuelos = resp.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<VMVuelo>>(ClienteHttpAuxiliar.ObtenerBody(resp))
                : new List<VMVuelo>();

            return View(vuelos);
        }

        // ── RF04: Alta (solo Administrador) ────────────────────────────────────

        [SoloAdminFilter]
        public IActionResult Create()
        {
            CargarSelectAeropuertos();
            return View(new VMCrearVuelo());
        }

        [HttpPost]
        [SoloAdminFilter]
        public IActionResult Create(VMCrearVuelo vm)
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "vuelo", VerbosHttp.POST, vm, Token);

            if (resp.IsSuccessStatusCode)
            {
                TempData["Exito"] = $"Vuelo {vm.Codigo} creado correctamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Error = ManejadorErroresApi.ObtenerMensajeError(resp);
            CargarSelectAeropuertos();
            return View(vm);
        }

        // ── RF04: Modificación (solo PROGRAMADO, solo Administrador) ───────────

        [SoloAdminFilter]
        public IActionResult Edit(int id)
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + $"vuelo/{id}", VerbosHttp.GET, null, Token);

            if (!resp.IsSuccessStatusCode) return NotFound();

            var vuelo = JsonConvert.DeserializeObject<VMVuelo>(ClienteHttpAuxiliar.ObtenerBody(resp));

            // La API devuelve los aeropuertos como texto ("MVD — Montevideo");
            // para editar necesitamos los IDs: se seleccionan de nuevo en el formulario.
            var vm = new VMModificarVuelo
            {
                Id = vuelo.Id,
                Codigo = vuelo.Codigo,
                FechaSalida = vuelo.FechaSalida,
                FechaLlegadaEstimada = vuelo.FechaLlegadaEstimada,
                PrecioBase = vuelo.PrecioBase,
                CapacidadAsientos = vuelo.CapacidadAsientos
            };

            CargarSelectAeropuertos();
            ViewBag.OrigenActual = vuelo.Origen;
            ViewBag.DestinoActual = vuelo.Destino;
            return View(vm);
        }

        [HttpPost]
        [SoloAdminFilter]
        public IActionResult Edit(VMModificarVuelo vm)
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + $"vuelo/{vm.Id}", VerbosHttp.PUT, vm, Token);

            if (resp.IsSuccessStatusCode)
            {
                TempData["Exito"] = $"Vuelo {vm.Codigo} modificado correctamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Error = ManejadorErroresApi.ObtenerMensajeError(resp);
            CargarSelectAeropuertos();
            return View(vm);
        }

        // ── RF04: Baja lógica (solo sin pasajes vendidos) ──────────────────────

        [HttpPost]
        [SoloAdminFilter]
        public IActionResult Delete(int id)
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + $"vuelo/{id}", VerbosHttp.DELETE, null, Token);

            if (resp.IsSuccessStatusCode)
                TempData["Exito"] = "Vuelo eliminado correctamente.";
            else
                TempData["Error"] = ManejadorErroresApi.ObtenerMensajeError(resp);

            return RedirectToAction("Index");
        }

        // ── RF05: Cambio de estado (Admin / Operador) ──────────────────────────

        [HttpPost]
        [AdminYOperadorFilter]
        public IActionResult CambiarEstado(int id, string nuevoEstado)
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + $"vuelo/{id}/estado", VerbosHttp.POST, new { NuevoEstado = nuevoEstado }, Token);

            if (resp.IsSuccessStatusCode)
            {
                TempData["Exito"] = nuevoEstado == "CANCELADO"
                    ? "Vuelo cancelado: se devolvió el 100 % a todos los pasajes vendidos."
                    : $"El vuelo pasó a {nuevoEstado}.";
            }
            else
            {
                TempData["Error"] = ManejadorErroresApi.ObtenerMensajeError(resp);
            }

            return RedirectToAction("Index");
        }

        // ── Helper: dropdown de aeropuertos ────────────────────────────────────

        private void CargarSelectAeropuertos()
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "aeropuerto", VerbosHttp.GET, null, Token);

            var aeropuertos = resp.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<VMAeropuerto>>(ClienteHttpAuxiliar.ObtenerBody(resp))
                : new List<VMAeropuerto>();

            ViewBag.Aeropuertos = aeropuertos
                .Select(a => new SelectListItem($"{a.CodigoIATA} — {a.Ciudad} ({a.Pais})", a.Id.ToString()))
                .ToList();
        }
    }
}
