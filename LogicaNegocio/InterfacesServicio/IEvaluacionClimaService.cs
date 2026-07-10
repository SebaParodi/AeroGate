using LogicaNegocio.Entidades;
using LogicaNegocio.Enumerados;

namespace LogicaNegocio.InterfacesServicio
{
    // RF13 (opcional): evaluación de condiciones del vuelo con IA + fallback por reglas.
    // La implementación vive en la capa de Infraestructura; el dominio solo conoce el contrato.
    public interface IEvaluacionClimaService
    {
        ResultadoEvaluacion EvaluarCondiciones(Vuelo vuelo);
    }
}
