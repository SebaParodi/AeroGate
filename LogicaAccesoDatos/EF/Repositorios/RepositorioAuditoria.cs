using System.Collections.Generic;
using System.Linq;
using LogicaNegocio.Entidades;
using LogicaNegocio.IRepositorios;
using Microsoft.EntityFrameworkCore;

namespace LogicaAccesoDatos.EF.Repositorios
{
    public class RepositorioAuditoria : IRepositorioAuditoria
    {
        private readonly AeroGateContext _context;

        public RepositorioAuditoria(AeroGateContext context)
        {
            _context = context;
        }

        // Solo alta: la auditoría es inmutable (no hay Update ni Remove)
        public void Add(Auditoria a)
        {
            _context.Auditorias.Add(a);
            _context.SaveChanges();
        }

        public Auditoria FindById(int id)
        {
            return _context.Auditorias
                .Include(a => a.Usuario)
                .SingleOrDefault(a => a.Id == id);
        }

        public List<Auditoria> GetAll()
        {
            return _context.Auditorias
                .Include(a => a.Usuario)
                .OrderByDescending(a => a.FechaHora)
                .ToList();
        }

        public List<Auditoria> GetByUsuario(int usuarioId)
        {
            return _context.Auditorias
                .Include(a => a.Usuario)
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.FechaHora)
                .ToList();
        }
    }
}
