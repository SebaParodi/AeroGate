using DTOs.DataTransferObjects.DTOsVuelo;

namespace LogicaAplicacion.ICasosDeUso.ICUVuelo
{
    public interface ICUCambiarEstadoVuelo
    {
        // Si el nuevo estado es CANCELADO, dispara la devolución automática del 100 %
        void Ejecutar(DTOCambiarEstadoVuelo dto);
    }
}
