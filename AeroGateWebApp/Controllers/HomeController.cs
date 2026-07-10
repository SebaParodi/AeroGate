using AeroGateWebApp.Auxiliar;
using AeroGateWebApp.Enums;
using AeroGateWebApp.Filter;
using AeroGateWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace AeroGateWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly string baseUrl;

        public HomeController(IConfiguration config)
            => baseUrl = config["ApiSettings:BaseUrl"];

        private string Token => HttpContext.Session.GetString("token");

        [LoginFilter]
        public IActionResult Index()
        {
            string rol = HttpContext.Session.GetString("rol");

            var vm = new VMDashboard
            {
                NombreUsuario = HttpContext.Session.GetString("nombre") ?? "",
                Rol           = rol ?? ""
            };

            if (rol == "ADMINISTRADOR")
            {
                vm.TotalVuelos      = GetLista<VMVuelo>("vuelo")?.Count ?? 0;
                vm.TotalAeropuertos = GetLista<VMAeropuerto>("aeropuerto")?.Count ?? 0;
                vm.TotalUsuarios    = GetLista<VMUsuario>("usuario")?.Count ?? 0;
            }
            else if (rol == "OPERADOR")
            {
                vm.TotalVuelos      = GetLista<VMVuelo>("vuelo")?.Count ?? 0;
                vm.TotalAeropuertos = GetLista<VMAeropuerto>("aeropuerto")?.Count ?? 0;
            }
            else if (rol == "CLIENTE")
            {
                vm.VuelosDisponibles = GetLista<VMVuelo>("vuelo/disponibles")?.Count ?? 0;
                vm.MisPasajesActivos = GetLista<VMPasaje>("pasaje/mis-pasajes")
                    ?.Count(p => p.Estado == "COMPRADO") ?? 0;
            }

            return View(vm);
        }

        // Helper genérico: GET + deserialización a lista de ViewModels
        private List<T> GetLista<T>(string recurso)
        {
            HttpResponseMessage resp = ClienteHttpAuxiliar.EnviarSolicitud(
                baseUrl + recurso, VerbosHttp.GET, null, Token);
            return resp.IsSuccessStatusCode
                ? JsonConvert.DeserializeObject<List<T>>(ClienteHttpAuxiliar.ObtenerBody(resp))
                : null;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new ErrorViewModel
                { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
