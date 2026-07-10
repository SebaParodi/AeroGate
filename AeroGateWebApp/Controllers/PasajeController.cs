using AeroGateWebApp.Auxiliar;
using AeroGateWebApp.Enums;
using AeroGateWebApp.Filter;
using AeroGateWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace AeroGateWebApp.Controllers
{
    public class PasajeController : Controller
    {
        private readonly string baseUrl;

        public PasajeController(IConfiguration config)
            => baseUrl = config["ApiSettings:BaseUrl"];

        private string Token => HttpContext.Session.GetString("token");

        // ── RF06: Comprar pasaje (Cliente) ─────────────────────────────────────

        [SoloClienteFilter]
        public IActionResult Comprar()
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "vuelo/disponibles", VerbosHttp.GET, null, Token);

            var vuelos = resp.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<VMVuelo>>(ClienteHttpAuxiliar.ObtenerBody(resp))
                : new List<VMVuelo>();

            return View(vuelos);
        }

        [HttpPost]
        [SoloClienteFilter]
        public IActionResult Comprar(int vueloId)
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "pasaje", VerbosHttp.POST, new { VueloId = vueloId }, Token);

            if (resp.IsSuccessStatusCode)
            {
                var pasaje = JsonConvert.DeserializeObject<VMPasaje>(ClienteHttpAuxiliar.ObtenerBody(resp));
                TempData["Exito"] = $"Compra confirmada: vuelo {pasaje.CodigoVuelo} por ${pasaje.PrecioPagado:0.00}" +
                    (pasaje.DescuentoAplicado > 0 ? $" (descuento {pasaje.DescuentoAplicado:P0} por tu categoría)." : ".");
                return RedirectToAction("MisPasajes");
            }

            TempData["Error"] = ManejadorErroresApi.ObtenerMensajeError(resp);
            return RedirectToAction("Comprar");
        }

        // ── RF08: Mis pasajes con filtro mes/año (Cliente) ─────────────────────

        [SoloClienteFilter]
        public IActionResult MisPasajes(int? mes, int? anio)
        {
            string q = "";
            if (mes.HasValue && anio.HasValue)
                q = $"?mes={mes}&anio={anio}";

            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "pasaje/mis-pasajes" + q, VerbosHttp.GET, null, Token);

            var pasajes = resp.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<VMPasaje>>(ClienteHttpAuxiliar.ObtenerBody(resp))
                : new List<VMPasaje>();

            if (!resp.IsSuccessStatusCode)
                ViewBag.Error = ManejadorErroresApi.ObtenerMensajeError(resp);

            ViewBag.Mes = mes;
            ViewBag.Anio = anio;
            return View(pasajes);
        }

        // ── RF07: Devolución con confirmación del monto (Cliente) ──────────────

        [SoloClienteFilter]
        public IActionResult ConfirmarDevolucion(int id)
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + $"pasaje/{id}/monto-devolucion", VerbosHttp.GET, null, Token);

            if (!resp.IsSuccessStatusCode)
            {
                TempData["Error"] = ManejadorErroresApi.ObtenerMensajeError(resp);
                return RedirectToAction("MisPasajes");
            }

            ViewBag.PasajeId = id;
            ViewBag.Monto = decimal.Parse(ClienteHttpAuxiliar.ObtenerBody(resp),
                System.Globalization.CultureInfo.InvariantCulture);
            return View();
        }

        [HttpPost]
        [SoloClienteFilter]
        public IActionResult Devolver(int id)
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + $"pasaje/{id}/devolucion", VerbosHttp.POST, null, Token);

            if (resp.IsSuccessStatusCode)
            {
                var pasaje = JsonConvert.DeserializeObject<VMPasaje>(ClienteHttpAuxiliar.ObtenerBody(resp));
                TempData["Exito"] = $"Devolución realizada: se reintegran ${pasaje.MontoReintegrado:0.00}.";
            }
            else
            {
                TempData["Error"] = ManejadorErroresApi.ObtenerMensajeError(resp);
            }

            return RedirectToAction("MisPasajes");
        }

        // ── RF09: Pasajeros por vuelo (Admin / Operador) ───────────────────────

        [AdminYOperadorFilter]
        public IActionResult PasajerosPorVuelo(int? vueloId)
        {
            // Dropdown de vuelos
            HttpResponseMessage rVuelos = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "vuelo", VerbosHttp.GET, null, Token);
            var vuelos = rVuelos.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<VMVuelo>>(ClienteHttpAuxiliar.ObtenerBody(rVuelos))
                : new List<VMVuelo>();

            ViewBag.Vuelos = vuelos
                .Select(v => new SelectListItem($"{v.Codigo}: {v.Origen} → {v.Destino}", v.Id.ToString(), v.Id == vueloId))
                .ToList();
            ViewBag.VueloId = vueloId;

            var pasajeros = new List<VMCliente>();
            if (vueloId.HasValue)
            {
                HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                    baseUrl + $"pasaje/pasajeros-por-vuelo/{vueloId}", VerbosHttp.GET, null, Token);

                if (resp.IsSuccessStatusCode)
                    pasajeros = JsonConvert.DeserializeObject<List<VMCliente>>(ClienteHttpAuxiliar.ObtenerBody(resp));
                else
                    ViewBag.Error = ManejadorErroresApi.ObtenerMensajeError(resp);
            }

            return View(pasajeros);
        }

        // ── RF10: Ranking de destinos (todos los roles) ────────────────────────

        [LoginFilter]
        public IActionResult Ranking()
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "pasaje/ranking-destinos", VerbosHttp.GET, null, Token);

            var ranking = resp.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<VMRankingDestino>>(ClienteHttpAuxiliar.ObtenerBody(resp))
                : new List<VMRankingDestino>();

            return View(ranking);
        }
    }
}
