using System.Collections.Generic;
using System.Linq;
using DTOs.DataTransferObjects.DTOsAeropuerto;
using LogicaNegocio.Entidades;

namespace DTOs.Mappers
{
    public class MapperAeropuerto
    {
        public static DTOAeropuerto ToDtoAeropuerto(Aeropuerto a)
        {
            return new DTOAeropuerto
            {
                Id = a.Id,
                CodigoIATA = a.CodigoIATA != null ? a.CodigoIATA.Valor : "",
                Nombre = a.Nombre,
                Ciudad = a.Ciudad,
                Pais = a.Pais
            };
        }

        public static List<DTOAeropuerto> ToListDtoAeropuerto(List<Aeropuerto> aeropuertos)
        {
            return aeropuertos.Select(a => ToDtoAeropuerto(a)).ToList();
        }
    }
}
