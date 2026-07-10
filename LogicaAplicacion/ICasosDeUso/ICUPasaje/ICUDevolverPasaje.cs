using DTOs.DataTransferObjects.DTOsPasaje;

namespace LogicaAplicacion.ICasosDeUso.ICUPasaje
{
    public interface ICUDevolverPasaje
    {
        // RF07: el sistema muestra el monto antes de confirmar
        decimal CalcularMonto(int pasajeId, int clienteId);
        DTOPasaje Ejecutar(DTODevolverPasaje dto);
    }
}
