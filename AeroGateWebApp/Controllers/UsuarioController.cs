using AeroGateWebApp.Auxiliar;
using AeroGateWebApp.Enums;
using AeroGateWebApp.Filter;
using AeroGateWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AeroGateWebApp.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly string baseUrl;

        public UsuarioController(IConfiguration config)
            => baseUrl = config["ApiSettings:BaseUrl"];

        private string Token => HttpContext.Session.GetString("token");

        // ── Listado de usuarios (solo Administrador) ───────────────────────────

        [SoloAdminFilter]
        public IActionResult Index()
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "usuario", VerbosHttp.GET, null, Token);

            var usuarios = resp.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<VMUsuario>>(ClienteHttpAuxiliar.ObtenerBody(resp))
                : new List<VMUsuario>();

            return View(usuarios);
        }

        // ── RF02: Alta de usuarios internos (solo Administrador) ───────────────

        [SoloAdminFilter]
        public IActionResult Create() => View(new VMAltaEmpleado());

        [HttpPost]
        [SoloAdminFilter]
        public IActionResult Create(VMAltaEmpleado vm)
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "usuario/empleados", VerbosHttp.POST, vm, Token);

            if (resp.IsSuccessStatusCode)
            {
                TempData["Exito"] = $"Empleado creado correctamente ({vm.Rol}).";
                return RedirectToAction("Index");
            }

            ViewBag.Error = ManejadorErroresApi.ObtenerMensajeError(resp);
            return View(vm);
        }
    }
}
