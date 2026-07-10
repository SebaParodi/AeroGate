using System;

namespace DTOs.DataTransferObjects.DTOsPasaje
{
    public class DTOPasaje
    {
        public int Id { get; set; }
        public string CodigoVuelo { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public DateTime FechaSalida { get; set; }
        public string ClienteNombre { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal PrecioPagado { get; set; }
        public decimal DescuentoAplicado { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public decimal? MontoReintegrado { get; set; }
    }
}
