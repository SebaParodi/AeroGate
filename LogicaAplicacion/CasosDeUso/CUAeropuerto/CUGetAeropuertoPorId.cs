using DTOs.DataTransferObjects.DTOsAeropuerto;
using DTOs.Mappers;
using LogicaAplicacion.ICasosDeUso.ICUAeropuerto;
using LogicaNegocio.Entidades;
using LogicaNegocio.Excepciones;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUAeropuerto
{
    public class CUGetAeropuertoPorId : ICUGetAeropuertoPorId
    {
        private readonly IRepositorioAeropuerto _repoAeropuerto;

        public CUGetAeropuertoPorId(IRepositorioAeropuerto repoAeropuerto)
        {
            _repoAeropuerto = repoAeropuerto;
        }

        public DTOAeropuerto Ejecutar(int id)
        {
            Aeropuerto aeropuerto = _repoAeropuerto.FindById(id);
            if (aeropuerto == null)
                throw new AeropuertoException($"No existe un aeropuerto con ID {id}");

            return MapperAeropuerto.ToDtoAeropuerto(aeropuerto);
        }
    }
}
