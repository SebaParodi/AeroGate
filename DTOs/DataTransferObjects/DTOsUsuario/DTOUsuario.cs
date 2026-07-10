namespace DTOs.DataTransferObjects.DTOsUsuario
{
    // Listado de usuarios para el Administrador
    public class DTOUsuario
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public string Categoria { get; set; }   // vacío si es empleado
    }
}
