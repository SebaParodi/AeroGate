using DTOs.DataTransferObjects.DTOsVuelo;

namespace LogicaAplicacion.ICasosDeUso.ICUVuelo
{
    public interface ICUGetVueloPorId
    {
        DTOVuelo Ejecutar(int id);
    }
}
