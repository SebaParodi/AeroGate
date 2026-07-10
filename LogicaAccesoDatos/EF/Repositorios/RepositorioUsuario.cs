using System.Collections.Generic;
using System.Linq;
using LogicaNegocio.Entidades;
using LogicaNegocio.IRepositorios;

namespace LogicaAccesoDatos.EF.Repositorios
{
    public class RepositorioUsuario : IRepositorioUsuario
    {
        private readonly AeroGateContext _context;

        public RepositorioUsuario(AeroGateContext context)
        {
            _context = context;
        }

        public void Add(Usuario u)
        {
            _context.Usuarios.Add(u);
            _context.SaveChanges();
        }

        public void Update(Usuario u)
        {
            _context.Usuarios.Update(u);
            _context.SaveChanges();
        }

        public Usuario FindById(int id)
        {
            return _context.Usuarios.SingleOrDefault(u => u.Id == id);
        }

        public Usuario FindByEmail(string email)
        {
            return _context.Usuarios.SingleOrDefault(u => u.Email == email);
        }

        public List<Usuario> GetAll()
        {
            return _context.Usuarios
                .OrderBy(u => u.NombreCompleto.Apellido)
                .ThenBy(u => u.NombreCompleto.Nombre)
                .ToList();
        }

        public List<Cliente> GetClientes()
        {
            return _context.Usuarios
                .OfType<Cliente>()
                .OrderBy(c => c.NombreCompleto.Apellido)
                .ThenBy(c => c.NombreCompleto.Nombre)
                .ToList();
        }
    }
}
