namespace DTOs.DataTransferObjects.DTOsPasaje
{
    // RF06: compra. El ClienteId sale del JWT en la API (el cliente compra para sí mismo)
    public class DTOComprarPasaje
    {
        public int ClienteId { get; set; }
        public int VueloId { get; set; }
    }
}
