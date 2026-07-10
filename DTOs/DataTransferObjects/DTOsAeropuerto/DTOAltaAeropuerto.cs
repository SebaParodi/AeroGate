namespace DTOs.DataTransferObjects.DTOsAeropuerto
{
    // RF12: alta de aeropuerto. UsuarioId = quién realiza la operación (para auditoría)
    public class DTOAltaAeropuerto
    {
        public string CodigoIATA { get; set; }
        public string Nombre { get; set; }
        public string Ciudad { get; set; }
        public string Pais { get; set; }
        public int UsuarioId { get; set; }
    }
}
