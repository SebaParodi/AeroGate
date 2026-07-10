namespace DTOs.DataTransferObjects.DTOsVuelo
{
    // RF05: cambio de estado (NuevoEstado: PROGRAMADO, DEMORADO, EMBARQUE, EN_VUELO, ATERRIZADO, CANCELADO)
    public class DTOCambiarEstadoVuelo
    {
        public int VueloId { get; set; }
        public string NuevoEstado { get; set; }
        public int UsuarioId { get; set; }
    }
}
