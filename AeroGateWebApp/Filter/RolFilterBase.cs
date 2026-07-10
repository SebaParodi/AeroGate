using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AeroGateWebApp.Filter
{
    /// <summary>
    /// Base de los filtros de autorización del MVC.
    /// Verifica que haya un token en sesión (usuario logueado) y, opcionalmente,
    /// que el rol guardado en sesión esté entre los permitidos.
    /// La autorización real la hace la API con el JWT: esto solo mejora la UX
    /// (redirige antes de mostrar una pantalla que la API va a rechazar).
    /// </summary>
    public abstract class RolFilterBase : ActionFilterAttribute
    {
        private readonly string[] _rolesPermitidos;

        protected RolFilterBase(params string[] rolesPermitidos)
        {
            _rolesPermitidos = rolesPermitidos;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ISession session = context.HttpContext.Session;
            string token = session.GetString("token");

            // Sin token → no logueado → al login
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }

            // Con roles requeridos → verificar el rol guardado en sesión
            if (_rolesPermitidos.Length > 0)
            {
                string rol = session.GetString("rol");
                if (string.IsNullOrEmpty(rol) || !_rolesPermitidos.Contains(rol))
                {
                    context.Result = new RedirectToActionResult("Index", "Home", null);
                }
            }
        }
    }
}
