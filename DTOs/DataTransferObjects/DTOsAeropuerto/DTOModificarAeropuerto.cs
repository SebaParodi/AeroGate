namespace DTOs.DataTransferObjects.DTOsAeropuerto
{
    // RF12: modificación. El código IATA es inmutable (identidad del catálogo)
    public class DTOModificarAeropuerto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Ciudad { get; set; }
        public string Pais { get; set; }
        public int UsuarioId { get; set; }
    }
}
