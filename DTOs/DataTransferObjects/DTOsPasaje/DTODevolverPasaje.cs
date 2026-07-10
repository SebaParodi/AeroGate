namespace DTOs.DataTransferObjects.DTOsPasaje
{
    // RF07: devolución voluntaria. El ClienteId sale del JWT (solo se devuelven pasajes propios)
    public class DTODevolverPasaje
    {
        public int PasajeId { get; set; }
        public int ClienteId { get; set; }
    }
}
