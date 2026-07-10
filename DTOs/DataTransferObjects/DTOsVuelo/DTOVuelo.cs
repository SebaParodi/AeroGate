using System;

namespace DTOs.DataTransferObjects.DTOsVuelo
{
    public class DTOVuelo
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Origen { get; set; }      // "MVD — Montevideo"
        public string Destino { get; set; }
        public DateTime FechaSalida { get; set; }
        public DateTime FechaLlegadaEstimada { get; set; }
        public decimal PrecioBase { get; set; }
        public int CapacidadAsientos { get; set; }
        public int AsientosDisponibles { get; set; }
        public string Estado { get; set; }
    }
}
