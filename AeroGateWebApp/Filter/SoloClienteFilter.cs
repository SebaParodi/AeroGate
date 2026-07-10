namespace AeroGateWebApp.Filter
{
    /// <summary>Solo Cliente (pasajero registrado).</summary>
    public class SoloClienteFilter : RolFilterBase
    {
        public SoloClienteFilter() : base("CLIENTE") { }
    }
}
