using System;
using System.Collections.Generic;
using System.Linq;
using DTOs.DataTransferObjects.DTOsPasaje;
using LogicaNegocio.Consultas;
using LogicaNegocio.Entidades;

namespace DTOs.Mappers
{
    public class MapperPasaje
    {
        public static DTOPasaje ToDtoPasaje(Pasaje p)
        {
            return new DTOPasaje
            {
                Id = p.Id,
                CodigoVuelo = p.Vuelo != null ? p.Vuelo.Codigo : "",
                Origen = p.Vuelo != null && p.Vuelo.AeropuertoOrigen != null
                    ? $"{p.Vuelo.AeropuertoOrigen.CodigoIATA} — {p.Vuelo.AeropuertoOrigen.Ciudad}" : "",
                Destino = p.Vuelo != null && p.Vuelo.AeropuertoDestino != null
                    ? $"{p.Vuelo.AeropuertoDestino.CodigoIATA} — {p.Vuelo.AeropuertoDestino.Ciudad}" : "",
                FechaSalida = p.Vuelo != null ? p.Vuelo.FechaSalida : default(DateTime),
                ClienteNombre = p.Cliente != null && p.Cliente.NombreCompleto != null
                    ? p.Cliente.NombreCompleto.ToString() : "",
                FechaCompra = p.FechaCompra,
                PrecioPagado = p.PrecioPagado,
                DescuentoAplicado = p.DescuentoAplicado,
                Estado = p.Estado.ToString(),
                FechaDevolucion = p.FechaDevolucion,
                MontoReintegrado = p.MontoReintegrado
            };
        }

        public static List<DTOPasaje> ToListDtoPasaje(List<Pasaje> pasajes)
        {
            return pasajes.Select(p => ToDtoPasaje(p)).ToList();
        }

        public static DTORankingDestino ToDtoRankingDestino(RankingDestino r)
        {
            return new DTORankingDestino
            {
                CodigoIATA = r.CodigoIATA,
                Ciudad = r.Ciudad,
                Pais = r.Pais,
                CantidadPasajes = r.CantidadPasajes
            };
        }

        public static List<DTORankingDestino> ToListDtoRankingDestino(List<RankingDestino> ranking)
        {
            return ranking.Select(r => ToDtoRankingDestino(r)).ToList();
        }
    }
}
