using AeroGateWebApp.Auxiliar;
using AeroGateWebApp.Enums;
using AeroGateWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AeroGateWebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly string baseUrl;

        public LoginController(IConfiguration config)
            => baseUrl = config["ApiSettings:BaseUrl"];

        // ── RF03: Login ────────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult Index(string email, string pass)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "auth/login", VerbosHttp.POST, new { Email = email, Pass = pass });

            if (!respuesta.IsSuccessStatusCode)
            {
                ViewBag.Error = ManejadorErroresApi.ObtenerMensajeError(respuesta);
                return View();
            }

            VMLoginResponse login = JsonConvert.DeserializeObject<VMLoginResponse>(
                ClienteHttpAuxiliar.ObtenerBody(respuesta));

            if (login == null || string.IsNullOrEmpty(login.Token))
            {
                ViewBag.Error = "Respuesta inválida del servidor.";
                return View();
            }

            // Guardar token + datos del usuario en sesión (los filtros y las vistas los leen de acá)
            HttpContext.Session.SetString("token", login.Token);
            HttpContext.Session.SetString("rol", login.Rol);
            HttpContext.Session.SetString("id", login.Id.ToString());
            HttpContext.Session.SetString("nombre", login.Nombre);

            return RedirectToAction("Index", "Home");
        }

        // ── RF01: Registro público de cliente ─────────────────────────────────

        [HttpGet]
        public IActionResult Registro() => View(new VMRegistroCliente());

        [HttpPost]
        public IActionResult Registro(VMRegistroCliente vm)
        {
            HttpResponseMessage respuesta = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + "usuario/registro", VerbosHttp.POST, vm);

            if (!respuesta.IsSuccessStatusCode)
            {
                ViewBag.Error = ManejadorErroresApi.ObtenerMensajeError(respuesta);
                return View(vm);
            }

            TempData["Exito"] = "Registro exitoso. Ya podés iniciar sesión.";
            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
