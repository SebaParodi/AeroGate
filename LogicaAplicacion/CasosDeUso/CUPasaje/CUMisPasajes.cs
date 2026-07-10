using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsPasaje;
using DTOs.Mappers;
using LogicaAplicacion.ICasosDeUso.ICUPasaje;
using LogicaNegocio.Entidades;
using LogicaNegocio.Excepciones;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUPasaje
{
    public class CUMisPasajes : ICUMisPasajes
    {
        private readonly IRepositorioPasaje _repoPasaje;

        public CUMisPasajes(IRepositorioPasaje repoPasaje)
        {
            _repoPasaje = repoPasaje;
        }

        // RF08: historial del cliente, filtrable por mes/año (ambos o ninguno)
        public List<DTOPasaje> Ejecutar(int clienteId, int? mes, int? anio)
        {
            if (mes.HasValue != anio.HasValue)
                throw new PasajeException("Para filtrar debe indicar mes y año juntos");

            List<Pasaje> pasajes;
            if (mes.HasValue)
            {
                if (mes.Value < 1 || mes.Value > 12) throw new PasajeException("El mes debe estar entre 1 y 12");
                pasajes = _repoPasaje.GetByClienteMesAnio(clienteId, mes.Value, anio.Value);
            }
            else
            {
                pasajes = _repoPasaje.GetByCliente(clienteId);
            }

            return MapperPasaje.ToListDtoPasaje(pasajes);
        }
    }
}
