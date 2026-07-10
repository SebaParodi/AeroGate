using System;
using System.Collections.Generic;
using System.Linq;
using LogicaNegocio.Consultas;
using LogicaNegocio.Entidades;
using LogicaNegocio.Enumerados;
using LogicaNegocio.IRepositorios;
using Microsoft.EntityFrameworkCore;

namespace LogicaAccesoDatos.EF.Repositorios
{
    public class RepositorioPasaje : IRepositorioPasaje
    {
        private readonly AeroGateContext _context;

        public RepositorioPasaje(AeroGateContext context)
        {
            _context = context;
        }

        public void Add(Pasaje p)
        {
            _context.Pasajes.Add(p);
            _context.SaveChanges();
        }

        public void Update(Pasaje p)
        {
            _context.Pasajes.Update(p);
            _context.SaveChanges();
        }

        public Pasaje FindById(int id)
        {
            return _context.Pasajes
                .Include(p => p.Cliente)
                .Include(p => p.Vuelo).ThenInclude(v => v.AeropuertoOrigen)
                .Include(p => p.Vuelo).ThenInclude(v => v.AeropuertoDestino)
                .SingleOrDefault(p => p.Id == id);
        }

        public List<Pasaje> GetByCliente(int clienteId)
        {
            return _context.Pasajes
                .Include(p => p.Vuelo).ThenInclude(v => v.AeropuertoOrigen)
                .Include(p => p.Vuelo).ThenInclude(v => v.AeropuertoDestino)
                .Where(p => p.ClienteId == clienteId)
                .OrderByDescending(p => p.FechaCompra)
                .ToList();
        }

        // RF08: mis pasajes de un mes/año específico
        public List<Pasaje> GetByClienteMesAnio(int clienteId, int mes, int anio)
        {
            return _context.Pasajes
                .Include(p => p.Vuelo).ThenInclude(v => v.AeropuertoOrigen)
                .Include(p => p.Vuelo).ThenInclude(v => v.AeropuertoDestino)
                .Where(p => p.ClienteId == clienteId
                         && p.FechaCompra.Month == mes
                         && p.FechaCompra.Year == anio)
                .OrderByDescending(p => p.FechaCompra)
                .ToList();
        }

        // Pasajes COMPRADOS de un vuelo: los que se devuelven al cancelar (RF05)
        public List<Pasaje> GetActivosByVuelo(int vueloId)
        {
            return _context.Pasajes
                .Where(p => p.VueloId == vueloId && p.Estado == EstadoPasaje.COMPRADO)
                .ToList();
        }

        // RF09: clientes distintos con pasaje comprado en el vuelo
        public List<Cliente> GetPasajerosPorVuelo(int vueloId)
        {
            return _context.Pasajes
                .Where(p => p.VueloId == vueloId && p.Estado == EstadoPasaje.COMPRADO)
                .Select(p => p.Cliente)
                .Distinct()
                .OrderBy(c => c.NombreCompleto.Apellido)
                .ThenBy(c => c.NombreCompleto.Nombre)
                .ToList();
        }

        // Recalculo de categoría: cuentan TODAS las compras, devueltas incluidas (LETRA 3.1)
        public int ContarComprasUltimos12Meses(int clienteId)
        {
            DateTime desde = DateTime.Now.AddMonths(-12);
            return _context.Pasajes.Count(p => p.ClienteId == clienteId && p.FechaCompra >= desde);
        }

        // RF10: destinos con más pasajes vendidos
        public List<RankingDestino> GetRankingDestinos()
        {
            return _context.Pasajes
                .GroupBy(p => new
                {
                    p.Vuelo.AeropuertoDestinoId,
                    Codigo = p.Vuelo.AeropuertoDestino.CodigoIATA.Valor,
                    p.Vuelo.AeropuertoDestino.Ciudad,
                    p.Vuelo.AeropuertoDestino.Pais
                })
                .Select(g => new RankingDestino
                {
                    AeropuertoId = g.Key.AeropuertoDestinoId,
                    CodigoIATA = g.Key.Codigo,
                    Ciudad = g.Key.Ciudad,
                    Pais = g.Key.Pais,
                    CantidadPasajes = g.Count()
                })
                .OrderByDescending(r => r.CantidadPasajes)
                .ToList();
        }
    }
}
