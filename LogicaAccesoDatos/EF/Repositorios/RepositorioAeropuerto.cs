using System.Collections.Generic;
using System.Linq;
using LogicaNegocio.Entidades;
using LogicaNegocio.IRepositorios;

namespace LogicaAccesoDatos.EF.Repositorios
{
    public class RepositorioAeropuerto : IRepositorioAeropuerto
    {
        private readonly AeroGateContext _context;

        public RepositorioAeropuerto(AeroGateContext context)
        {
            _context = context;
        }

        public void Add(Aeropuerto a)
        {
            _context.Aeropuertos.Add(a);
            _context.SaveChanges();
        }

        public void Update(Aeropuerto a)
        {
            _context.Aeropuertos.Update(a);
            _context.SaveChanges();
        }

        public Aeropuerto FindById(int id)
        {
            return _context.Aeropuertos.SingleOrDefault(a => a.Id == id);
        }

        public Aeropuerto FindByCodigoIATA(string codigo)
        {
            return _context.Aeropuertos.SingleOrDefault(a => a.CodigoIATA.Valor == codigo);
        }

        public List<Aeropuerto> GetAll(string filtro)
        {
            IQueryable<Aeropuerto> query = _context.Aeropuertos;

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(a => a.CodigoIATA.Valor.Contains(filtro)
                                      || a.Ciudad.Contains(filtro)
                                      || a.Pais.Contains(filtro));
            }

            return query.OrderBy(a => a.CodigoIATA.Valor).ToList();
        }
    }
}
