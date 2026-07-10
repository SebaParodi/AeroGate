using System;

namespace DTOs.DataTransferObjects.DTOsVuelo
{
    // RF04: modificación (solo vuelos PROGRAMADO). El código de vuelo es inmutable
    public class DTOModificarVuelo
    {
        public int Id { get; set; }
        public int AeropuertoOrigenId { get; set; }
        public int AeropuertoDestinoId { get; set; }
        public DateTime FechaSalida { get; set; }
        public DateTime FechaLlegadaEstimada { get; set; }
        public decimal PrecioBase { get; set; }
        public int CapacidadAsientos { get; set; }
        public int UsuarioId { get; set; }
    }
}
