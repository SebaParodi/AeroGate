using System.Collections.Generic;
using LogicaNegocio.Entidades;

namespace LogicaNegocio.IRepositorios
{
    public interface IRepositorioUsuario
    {
        void Add(Usuario u);
        void Update(Usuario u);                 // persiste el recálculo de categoría del cliente
        Usuario FindById(int id);
        Usuario FindByEmail(string email);      // login + control de unicidad
        List<Usuario> GetAll();                 // gestión de usuarios (Admin)
        List<Cliente> GetClientes();
    }
}
