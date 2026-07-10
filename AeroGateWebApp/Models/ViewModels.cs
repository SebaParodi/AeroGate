namespace AeroGateWebApp.Models
{
    // ── Login / Sesión ────────────────────────────────────────────────────────
    public class VMLoginResponse
    {
        public string Token    { get; set; } = "";
        public DateTime Expira { get; set; }
        public int Id          { get; set; }
        public string Nombre   { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string Email    { get; set; } = "";
        public string Rol      { get; set; } = "";
    }

    public class VMRegistroCliente
    {
        public string Nombre    { get; set; } = "";
        public string Apellido  { get; set; } = "";
        public string Email     { get; set; } = "";
        public string Pass      { get; set; } = "";
        public string Documento { get; set; } = "";
        public string Direccion { get; set; } = "";
    }

    // ── Dashboard ─────────────────────────────────────────────────────────────
    public class VMDashboard
    {
        public string NombreUsuario { get; set; } = "";
        public string Rol           { get; set; } = "";
        // Conteos agregados según el rol
        public int TotalVuelos       { get; set; }
        public int TotalAeropuertos  { get; set; }
        public int TotalUsuarios     { get; set; }
        public int VuelosDisponibles { get; set; }
        public int MisPasajesActivos { get; set; }
    }

    // ── Usuario ───────────────────────────────────────────────────────────────
    public class VMUsuario
    {
        public int Id                { get; set; }
        public string NombreCompleto { get; set; } = "";
        public string Email          { get; set; } = "";
        public string Rol            { get; set; } = "";
        public string Categoria      { get; set; } = "";
    }

    public class VMAltaEmpleado
    {
        public string Nombre        { get; set; } = "";
        public string Apellido      { get; set; } = "";
        public string Email         { get; set; } = "";
        public string Pass          { get; set; } = "";
        public string Rol           { get; set; } = "";
        public DateTime FechaIngreso { get; set; } = DateTime.Today;
    }

    public class VMCliente
    {
        public int Id                { get; set; }
        public string NombreCompleto { get; set; } = "";
        public string Email          { get; set; } = "";
        public string Documento      { get; set; } = "";
        public string Direccion      { get; set; } = "";
        public string Categoria      { get; set; } = "";
    }

    // ── Aeropuerto ────────────────────────────────────────────────────────────
    public class VMAeropuerto
    {
        public int Id            { get; set; }
        public string CodigoIATA { get; set; } = "";
        public string Nombre     { get; set; } = "";
        public string Ciudad     { get; set; } = "";
        public string Pais       { get; set; } = "";
    }

    public class VMCrearAeropuerto
    {
        public string CodigoIATA { get; set; } = "";
        public string Nombre     { get; set; } = "";
        public string Ciudad     { get; set; } = "";
        public string Pais       { get; set; } = "";
    }

    // ── Vuelo ─────────────────────────────────────────────────────────────────
    public class VMVuelo
    {
        public int Id                       { get; set; }
        public string Codigo                { get; set; } = "";
        public string Origen                { get; set; } = "";
        public string Destino               { get; set; } = "";
        public DateTime FechaSalida         { get; set; }
        public DateTime FechaLlegadaEstimada { get; set; }
        public decimal PrecioBase           { get; set; }
        public int CapacidadAsientos        { get; set; }
        public int AsientosDisponibles      { get; set; }
        public string Estado                { get; set; } = "";

        // Transiciones válidas de la máquina de estados (para los botones de la vista)
        public List<string> TransicionesValidas()
        {
            switch (Estado)
            {
                case "PROGRAMADO": return new List<string> { "DEMORADO", "EMBARQUE", "CANCELADO" };
                case "DEMORADO":   return new List<string> { "PROGRAMADO", "EMBARQUE", "CANCELADO" };
                case "EMBARQUE":   return new List<string> { "EN_VUELO", "CANCELADO" };
                case "EN_VUELO":   return new List<string> { "ATERRIZADO" };
                default:           return new List<string>();   // ATERRIZADO / CANCELADO: finales
            }
        }
    }

    public class VMCrearVuelo
    {
        public string Codigo             { get; set; } = "";
        public int AeropuertoOrigenId    { get; set; }
        public int AeropuertoDestinoId   { get; set; }
        public DateTime FechaSalida      { get; set; } = DateTime.Today.AddDays(1).AddHours(8);
        public DateTime FechaLlegadaEstimada { get; set; } = DateTime.Today.AddDays(1).AddHours(10);
        public decimal PrecioBase        { get; set; }
        public int CapacidadAsientos     { get; set; }
    }

    public class VMModificarVuelo
    {
        public int Id                    { get; set; }
        public string Codigo             { get; set; } = "";   // solo lectura en la vista
        public int AeropuertoOrigenId    { get; set; }
        public int AeropuertoDestinoId   { get; set; }
        public DateTime FechaSalida      { get; set; }
        public DateTime FechaLlegadaEstimada { get; set; }
        public decimal PrecioBase        { get; set; }
        public int CapacidadAsientos     { get; set; }
    }

    // ── Pasaje ────────────────────────────────────────────────────────────────
    public class VMPasaje
    {
        public int Id                     { get; set; }
        public string CodigoVuelo         { get; set; } = "";
        public string Origen              { get; set; } = "";
        public string Destino             { get; set; } = "";
        public DateTime FechaSalida       { get; set; }
        public string ClienteNombre       { get; set; } = "";
        public DateTime FechaCompra       { get; set; }
        public decimal PrecioPagado       { get; set; }
        public decimal DescuentoAplicado  { get; set; }
        public string Estado              { get; set; } = "";
        public DateTime? FechaDevolucion  { get; set; }
        public decimal? MontoReintegrado  { get; set; }
    }

    public class VMRankingDestino
    {
        public string CodigoIATA   { get; set; } = "";
        public string Ciudad       { get; set; } = "";
        public string Pais         { get; set; } = "";
        public int CantidadPasajes { get; set; }
    }

    // ── Auditoría ─────────────────────────────────────────────────────────────
    public class VMAuditoria
    {
        public int Id               { get; set; }
        public string UsuarioNombre { get; set; } = "";
        public string UsuarioEmail  { get; set; } = "";
        public string Operacion     { get; set; } = "";
        public string Detalle       { get; set; } = "";
        public DateTime FechaHora   { get; set; }
        public int? VueloId         { get; set; }
        public int? PasajeId        { get; set; }
        public int? AeropuertoId    { get; set; }
    }

    // ── Error ─────────────────────────────────────────────────────────────────
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
