using System.Collections.Generic;
using LogicaNegocio.Entidades;

namespace LogicaNegocio.IRepositorios
{
    public interface IRepositorioAeropuerto
    {
        void Add(Aeropuerto a);
        void Update(Aeropuerto a);                    // modificación (no hay baja)
        Aeropuerto FindById(int id);
        Aeropuerto FindByCodigoIATA(string codigo);   // control de unicidad
        List<Aeropuerto> GetAll(string filtro);       // búsqueda por código/ciudad/país (filtro null = todos)
    }
}
