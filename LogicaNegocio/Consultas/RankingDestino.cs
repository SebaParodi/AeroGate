namespace LogicaNegocio.Consultas
{
    // Resultado de consulta para el ranking de destinos (RF10). No es una entidad persistida.
    public class RankingDestino
    {
        public int AeropuertoId { get; set; }
        public string CodigoIATA { get; set; }
        public string Ciudad { get; set; }
        public string Pais { get; set; }
        public int CantidadPasajes { get; set; }
    }
}
