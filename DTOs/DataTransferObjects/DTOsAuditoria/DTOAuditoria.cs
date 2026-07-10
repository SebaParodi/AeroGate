using System;

namespace DTOs.DataTransferObjects.DTOsAuditoria
{
    // RF11: registro de auditoría (quién, qué, cuándo, sobre qué entidad)
    public class DTOAuditoria
    {
        public int Id { get; set; }
        public string UsuarioNombre { get; set; }
        public string UsuarioEmail { get; set; }
        public string Operacion { get; set; }
        public string Detalle { get; set; }
        public DateTime FechaHora { get; set; }
        public int? VueloId { get; set; }
        public int? PasajeId { get; set; }
        public int? AeropuertoId { get; set; }
    }
}
