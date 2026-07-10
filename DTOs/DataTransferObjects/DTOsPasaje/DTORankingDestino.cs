namespace DTOs.DataTransferObjects.DTOsPasaje
{
    // RF10: ranking de destinos con más pasajes vendidos
    public class DTORankingDestino
    {
        public string CodigoIATA { get; set; }
        public string Ciudad { get; set; }
        public string Pais { get; set; }
        public int CantidadPasajes { get; set; }
    }
}
