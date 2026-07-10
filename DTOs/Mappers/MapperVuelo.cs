using System.Collections.Generic;
using System.Linq;
using DTOs.DataTransferObjects.DTOsVuelo;
using LogicaNegocio.Entidades;

namespace DTOs.Mappers
{
    public class MapperVuelo
    {
        public static DTOVuelo ToDtoVuelo(Vuelo v)
        {
            return new DTOVuelo
            {
                Id = v.Id,
                Codigo = v.Codigo,
                Origen = FormatearAeropuerto(v.AeropuertoOrigen),
                Destino = FormatearAeropuerto(v.AeropuertoDestino),
                FechaSalida = v.FechaSalida,
                FechaLlegadaEstimada = v.FechaLlegadaEstimada,
                PrecioBase = v.PrecioBase,
                CapacidadAsientos = v.CapacidadAsientos,
                AsientosDisponibles = v.AsientosDisponibles,
                Estado = v.Estado.ToString()
            };
        }

        public static List<DTOVuelo> ToListDtoVuelo(List<Vuelo> vuelos)
        {
            return vuelos.Select(v => ToDtoVuelo(v)).ToList();
        }

        private static string FormatearAeropuerto(Aeropuerto a)
        {
            if (a == null) return "";
            return $"{a.CodigoIATA} — {a.Ciudad}";
        }
    }
}
