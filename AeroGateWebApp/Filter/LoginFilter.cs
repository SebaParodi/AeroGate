namespace AeroGateWebApp.Filter
{
    /// <summary>Permite el acceso a cualquier usuario logueado (token en sesión).</summary>
    public class LoginFilter : RolFilterBase
    {
        public LoginFilter() : base() { }
    }
}
