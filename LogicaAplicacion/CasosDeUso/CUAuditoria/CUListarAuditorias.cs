using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsAuditoria;
using DTOs.Mappers;
using LogicaAplicacion.ICasosDeUso.ICUAuditoria;
using LogicaNegocio.Entidades;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUAuditoria
{
    public class CUListarAuditorias : ICUListarAuditorias
    {
        private readonly IRepositorioAuditoria _repoAuditoria;

        public CUListarAuditorias(IRepositorioAuditoria repoAuditoria)
        {
            _repoAuditoria = repoAuditoria;
        }

        // RF11: el Administrador ve todas (usuarioId null), el Operador solo las suyas
        public List<DTOAuditoria> Ejecutar(int? usuarioId)
        {
            List<Auditoria> auditorias = usuarioId.HasValue
                ? _repoAuditoria.GetByUsuario(usuarioId.Value)
                : _repoAuditoria.GetAll();

            return MapperAuditoria.ToListDtoAuditoria(auditorias);
        }
    }
}
