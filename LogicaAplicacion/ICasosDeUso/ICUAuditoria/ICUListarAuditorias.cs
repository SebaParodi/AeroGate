using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsAuditoria;

namespace LogicaAplicacion.ICasosDeUso.ICUAuditoria
{
    public interface ICUListarAuditorias
    {
        // RF11: usuarioId null = todas (Administrador); con valor = solo las de ese usuario (Operador)
        List<DTOAuditoria> Ejecutar(int? usuarioId);
    }
}
