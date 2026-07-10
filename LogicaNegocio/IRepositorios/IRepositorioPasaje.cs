using System.Collections.Generic;
using LogicaNegocio.Consultas;
using LogicaNegocio.Entidades;

namespace LogicaNegocio.IRepositorios
{
    public interface IRepositorioPasaje
    {
        void Add(Pasaje p);
        void Update(Pasaje p);                  // devoluciones
        Pasaje FindById(int id);
        List<Pasaje> GetByCliente(int clienteId);
        List<Pasaje> GetByClienteMesAnio(int clienteId, int mes, int anio);  // RF08 mis pasajes
        List<Pasaje> GetActivosByVuelo(int vueloId);          // devolución automática al cancelar
        List<Cliente> GetPasajerosPorVuelo(int vueloId);      // RF09 pasajeros de un vuelo
        int ContarComprasUltimos12Meses(int clienteId);       // recálculo de categoría
        List<RankingDestino> GetRankingDestinos();            // RF10 ranking de destinos
    }
}
