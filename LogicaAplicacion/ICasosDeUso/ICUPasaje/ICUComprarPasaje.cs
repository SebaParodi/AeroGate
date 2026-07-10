using DTOs.DataTransferObjects.DTOsPasaje;

namespace LogicaAplicacion.ICasosDeUso.ICUPasaje
{
    public interface ICUComprarPasaje
    {
        DTOPasaje Ejecutar(DTOComprarPasaje dto);
    }
}
