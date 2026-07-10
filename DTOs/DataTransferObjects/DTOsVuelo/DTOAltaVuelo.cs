using System;

namespace DTOs.DataTransferObjects.DTOsVuelo
{
    // RF04: alta de vuelo. UsuarioId = quién realiza la operación (para auditoría)
    public class DTOAltaVuelo
    {
        public string Codigo { get; set; }
        public int AeropuertoOrigenId { get; set; }
        public int AeropuertoDestinoId { get; set; }
        public DateTime FechaSalida { get; set; }
        public DateTime FechaLlegadaEstimada { get; set; }
        public decimal PrecioBase { get; set; }
        public int CapacidadAsientos { get; set; }
        public int UsuarioId { get; set; }
    }
}
