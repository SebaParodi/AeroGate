using System;
using LogicaNegocio.Enumerados;
using LogicaNegocio.Excepciones;
using LogicaNegocio.InterfacesDominio;

namespace LogicaNegocio.Entidades
{
    public class Pasaje : IValidable
    {
        public int Id { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public int VueloId { get; set; }
        public Vuelo Vuelo { get; set; }

        public DateTime FechaCompra { get; set; }

        // Foto histórica: si el cliente cambia de categoría después, la compra no se recalcula
        public decimal PrecioPagado { get; set; }
        public decimal DescuentoAplicado { get; set; }

        public EstadoPasaje Estado { get; set; }
        public DateTime? FechaDevolucion { get; set; }
        public decimal? MontoReintegrado { get; set; }

        public Pasaje() { }

        public Pasaje(int clienteId, int vueloId, decimal precioPagado, decimal descuentoAplicado)
        {
            ClienteId = clienteId;
            VueloId = vueloId;
            FechaCompra = DateTime.Now;
            PrecioPagado = precioPagado;
            DescuentoAplicado = descuentoAplicado;
            Estado = EstadoPasaje.COMPRADO;
        }

        public void Validar()
        {
            if (ClienteId <= 0) throw new PasajeException("El cliente es obligatorio");
            if (VueloId <= 0) throw new PasajeException("El vuelo es obligatorio");
            if (PrecioPagado <= 0) throw new PasajeException("El precio pagado debe ser mayor a cero");
            if (DescuentoAplicado < 0 || DescuentoAplicado > 1) throw new PasajeException("El descuento aplicado debe estar entre 0 y 1");
        }

        // Devolución voluntaria: solo pasajes comprados de vuelos que aún no embarcaron (LETRA 3.5)
        public bool EsDevolvible()
        {
            if (Estado != EstadoPasaje.COMPRADO) 
                return false;
            if (Vuelo is null) 
                throw new PasajeException("El pasaje no tiene el vuelo cargado");
            
            return Vuelo.Estado == EstadoVuelo.PROGRAMADO || Vuelo.Estado == EstadoVuelo.DEMORADO;
        }

        public void Devolver(decimal montoReintegro)
        {
            if (Estado == EstadoPasaje.DEVUELTO) 
                throw new PasajeException("El pasaje ya fue devuelto");
            if (montoReintegro < 0) 
                throw new PasajeException("El monto a reintegrar no puede ser negativo");

            Estado = EstadoPasaje.DEVUELTO;
            FechaDevolucion = DateTime.Now;
            MontoReintegrado = montoReintegro;
        }
    }
}
