namespace DTOs.DataTransferObjects.DTOsUsuario
{
    // Cliente para listados (RF09 pasajeros por vuelo, gestión del admin)
    public class DTOCliente
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Documento { get; set; }
        public string Direccion { get; set; }
        public string Categoria { get; set; }
    }
}
