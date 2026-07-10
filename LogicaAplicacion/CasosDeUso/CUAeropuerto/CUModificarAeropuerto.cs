using DTOs.DataTransferObjects.DTOsAeropuerto;
using LogicaAplicacion.ICasosDeUso.ICUAeropuerto;
using LogicaAplicacion.ICasosDeUso.ICUAuditoria;
using LogicaNegocio.Entidades;
using LogicaNegocio.Enumerados;
using LogicaNegocio.Excepciones;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUAeropuerto
{
    public class CUModificarAeropuerto : ICUModificarAeropuerto
    {
        private readonly IRepositorioAeropuerto _repoAeropuerto;
        private readonly ICUGenerarAuditoria _cuAuditoria;

        public CUModificarAeropuerto(IRepositorioAeropuerto repoAeropuerto, ICUGenerarAuditoria cuAuditoria)
        {
            _repoAeropuerto = repoAeropuerto;
            _cuAuditoria = cuAuditoria;
        }

        public void Ejecutar(DTOModificarAeropuerto dto)
        {
            Aeropuerto aeropuerto = _repoAeropuerto.FindById(dto.Id);
            if (aeropuerto == null)
                throw new AeropuertoException($"No existe un aeropuerto con ID {dto.Id}");

            // El código IATA es inmutable: solo se modifican los datos descriptivos
            aeropuerto.Nombre = dto.Nombre;
            aeropuerto.Ciudad = dto.Ciudad;
            aeropuerto.Pais = dto.Pais;

            aeropuerto.Validar();
            _repoAeropuerto.Update(aeropuerto);

            _cuAuditoria.Ejecutar(dto.UsuarioId, TipoOperacion.MODIFICACION_AEROPUERTO,
                $"Modificación de aeropuerto {aeropuerto.CodigoIATA} — {aeropuerto.Ciudad}, {aeropuerto.Pais}",
                aeropuertoId: aeropuerto.Id);
        }
    }
}
