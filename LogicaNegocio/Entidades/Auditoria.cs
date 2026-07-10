using System;
using LogicaNegocio.Enumerados;
using LogicaNegocio.Excepciones;
using LogicaNegocio.InterfacesDominio;

namespace LogicaNegocio.Entidades
{
    // Registro inmutable: solo se da de alta, nunca se modifica ni elimina (LETRA 3.6)
    public class Auditoria : IValidable
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public TipoOperacion Operacion { get; set; }
        public string Detalle { get; set; }
        public DateTime FechaHora { get; set; }

        // Entidad afectada (solo aplica la que corresponda a la operación)
        public int? VueloId { get; set; }
        public int? PasajeId { get; set; }
        public int? AeropuertoId { get; set; }

        public Auditoria() { }

        public Auditoria(int usuarioId, TipoOperacion operacion, string detalle,
                         int? vueloId = null, int? pasajeId = null, int? aeropuertoId = null)
        {
            UsuarioId = usuarioId;
            Operacion = operacion;
            Detalle = detalle;
            FechaHora = DateTime.Now;
            VueloId = vueloId;
            PasajeId = pasajeId;
            AeropuertoId = aeropuertoId;
        }

        public void Validar()
        {
            if (UsuarioId <= 0) throw new AuditoriaException("El usuario que realizó la operación es obligatorio");
            if (string.IsNullOrWhiteSpace(Detalle)) throw new AuditoriaException("El detalle de la operación es obligatorio");
        }
    }
}
