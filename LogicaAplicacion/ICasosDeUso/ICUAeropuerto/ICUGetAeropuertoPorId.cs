using DTOs.DataTransferObjects.DTOsAeropuerto;

namespace LogicaAplicacion.ICasosDeUso.ICUAeropuerto
{
    public interface ICUGetAeropuertoPorId
    {
        DTOAeropuerto Ejecutar(int id);
    }
}
