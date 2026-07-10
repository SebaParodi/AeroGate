using System.Collections.Generic;
using System.Linq;
using DTOs.DataTransferObjects.DTOsAuditoria;
using LogicaNegocio.Entidades;

namespace DTOs.Mappers
{
    public class MapperAuditoria
    {
        public static DTOAuditoria ToDtoAuditoria(Auditoria a)
        {
            return new DTOAuditoria
            {
                Id = a.Id,
                UsuarioNombre = a.Usuario != null && a.Usuario.NombreCompleto != null
                    ? a.Usuario.NombreCompleto.ToString() : "",
                UsuarioEmail = a.Usuario != null ? a.Usuario.Email : "",
                Operacion = a.Operacion.ToString(),
                Detalle = a.Detalle,
                FechaHora = a.FechaHora,
                VueloId = a.VueloId,
                PasajeId = a.PasajeId,
                AeropuertoId = a.AeropuertoId
            };
        }

        public static List<DTOAuditoria> ToListDtoAuditoria(List<Auditoria> auditorias)
        {
            return auditorias.Select(a => ToDtoAuditoria(a)).ToList();
        }
    }
}
