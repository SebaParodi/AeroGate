using System;
using System.Collections.Generic;
using System.Linq;
using LogicaNegocio.Entidades;
using LogicaNegocio.Enumerados;
using LogicaNegocio.IRepositorios;
using Microsoft.EntityFrameworkCore;

namespace LogicaAccesoDatos.EF.Repositorios
{
    public class RepositorioVuelo : IRepositorioVuelo
    {
        private readonly AeroGateContext _context;

        public RepositorioVuelo(AeroGateContext context)
        {
            _context = context;
        }

        public void Add(Vuelo v)
        {
            _context.Vuelos.Add(v);
            _context.SaveChanges();
        }

        public void Update(Vuelo v)
        {
            _context.Vuelos.Update(v);
            _context.SaveChanges();
        }

        public Vuelo FindById(int id)
        {
            return _context.Vuelos
                .Include(v => v.AeropuertoOrigen)
                .Include(v => v.AeropuertoDestino)
                .SingleOrDefault(v => v.Id == id);
        }

        public Vuelo FindByCodigo(string codigo)
        {
            return _context.Vuelos.SingleOrDefault(v => v.Codigo == codigo);
        }

        public List<Vuelo> GetAll()
        {
            return _context.Vuelos
                .Include(v => v.AeropuertoOrigen)
                .Include(v => v.AeropuertoDestino)
                .Where(v => !v.Eliminado)
                .OrderBy(v => v.FechaSalida)
                .ToList();
        }

        public List<Vuelo> GetByAeropuerto(int aeropuertoId)
        {
            return _context.Vuelos
                .Include(v => v.AeropuertoOrigen)
                .Include(v => v.AeropuertoDestino)
                .Where(v => !v.Eliminado
                         && (v.AeropuertoOrigenId == aeropuertoId || v.AeropuertoDestinoId == aeropuertoId))
                .OrderBy(v => v.FechaSalida)
                .ToList();
        }

        // RF06: lo que ve el cliente para comprar (con salida futura)
        public List<Vuelo> GetDisponiblesParaVenta()
        {
            return _context.Vuelos
                .Include(v => v.AeropuertoOrigen)
                .Include(v => v.AeropuertoDestino)
                .Where(v => !v.Eliminado
                         && v.AsientosDisponibles > 0
                         && (v.Estado == EstadoVuelo.PROGRAMADO || v.Estado == EstadoVuelo.DEMORADO)
                         && v.FechaSalida > DateTime.Now)
                .OrderBy(v => v.FechaSalida)
                .ToList();
        }
    }
}
