namespace LogicaAplicacion.ICasosDeUso.ICUVuelo
{
    public interface ICUEliminarVuelo
    {
        // Baja lógica: solo vuelos sin pasajes vendidos (con ventas se cancela, no se elimina)
        void Ejecutar(int vueloId, int usuarioId);
    }
}
