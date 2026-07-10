using DTOs.DataTransferObjects.DTOsAeropuerto;
using LogicaAplicacion.ICasosDeUso.ICUAeropuerto;
using LogicaAplicacion.ICasosDeUso.ICUAuditoria;
using LogicaNegocio.Entidades;
using LogicaNegocio.Enumerados;
using LogicaNegocio.Excepciones;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUAeropuerto
{
    public class CUAltaAeropuerto : ICUAltaAeropuerto
    {
        private readonly IRepositorioAeropuerto _repoAeropuerto;
        private readonly ICUGenerarAuditoria _cuAuditoria;

        public CUAltaAeropuerto(IRepositorioAeropuerto repoAeropuerto, ICUGenerarAuditoria cuAuditoria)
        {
            _repoAeropuerto = repoAeropuerto;
            _cuAuditoria = cuAuditoria;
        }

        public void Ejecutar(DTOAltaAeropuerto dto)
        {
            // 1. Crear y validar la entidad (formato IATA, campos obligatorios)
            Aeropuerto aeropuerto = new Aeropuerto(dto.CodigoIATA, dto.Nombre, dto.Ciudad, dto.Pais);
            aeropuerto.Validar();

            // 2. Unicidad del código IATA en el catálogo
            if (_repoAeropuerto.FindByCodigoIATA(dto.CodigoIATA) != null)
                throw new AeropuertoException($"Ya existe un aeropuerto con el código {dto.CodigoIATA}");

            // 3. Persistir + auditoría automática (RF12)
            _repoAeropuerto.Add(aeropuerto);
            _cuAuditoria.Ejecutar(dto.UsuarioId, TipoOperacion.ALTA_AEROPUERTO,
                $"Alta de aeropuerto {dto.CodigoIATA} — {dto.Ciudad}, {dto.Pais}",
                aeropuertoId: aeropuerto.Id);
        }
    }
}
