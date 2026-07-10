using System.Collections.Generic;
using LogicaNegocio.Entidades;

namespace LogicaNegocio.IRepositorios
{
    public interface IRepositorioVuelo
    {
        void Add(Vuelo v);
        void Update(Vuelo v);                   // modificación, cambio de estado y baja lógica
        Vuelo FindById(int id);
        Vuelo FindByCodigo(string codigo);      // control de código único
        List<Vuelo> GetAll();                   // listado del admin
        List<Vuelo> GetByAeropuerto(int aeropuertoId);   // filtro por origen/destino
        List<Vuelo> GetDisponiblesParaVenta();  // PROGRAMADO o DEMORADO, con asientos, no eliminados
    }
}
