namespace DTOs.DataTransferObjects.DTOsUsuario
{
    // RF01: registro público de cliente (arranca en categoría ESTANDAR)
    public class DTORegistrarCliente
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public string Documento { get; set; }
        public string Direccion { get; set; }
    }
}
