namespace AeroGateWebApp.Filter
{
    /// <summary>Administrador u Operador (personal interno).</summary>
    public class AdminYOperadorFilter : RolFilterBase
    {
        public AdminYOperadorFilter() : base("ADMINISTRADOR", "OPERADOR") { }
    }
}
