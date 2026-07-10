using LogicaNegocio.Enumerados;

namespace LogicaAplicacion.ICasosDeUso.ICUAuditoria
{
    // CU interno: lo invocan los demás casos de uso para auditar operaciones críticas
    public interface ICUGenerarAuditoria
    {
        void Ejecutar(int usuarioId, TipoOperacion operacion, string detalle,
                      int? vueloId = null, int? pasajeId = null, int? aeropuertoId = null);
    }
}
