using LogicaAplicacion.ICasosDeUso.ICUAuditoria;
using LogicaNegocio.Entidades;
using LogicaNegocio.Enumerados;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUAuditoria
{
    // CU interno: los demás casos de uso lo componen para auditar operaciones críticas (LETRA 3.6)
    public class CUGenerarAuditoria : ICUGenerarAuditoria
    {
        private readonly IRepositorioAuditoria _repoAuditoria;

        public CUGenerarAuditoria(IRepositorioAuditoria repoAuditoria)
        {
            _repoAuditoria = repoAuditoria;
        }

        public void Ejecutar(int usuarioId, TipoOperacion operacion, string detalle,
                             int? vueloId = null, int? pasajeId = null, int? aeropuertoId = null)
        {
            Auditoria auditoria = new Auditoria(usuarioId, operacion, detalle, vueloId, pasajeId, aeropuertoId);
            auditoria.Validar();
            _repoAuditoria.Add(auditoria);
        }
    }
}
