using System;
using System.Text.RegularExpressions;
using LogicaNegocio.Enumerados;
using LogicaNegocio.Excepciones;
using LogicaNegocio.InterfacesDominio;

namespace LogicaNegocio.Entidades
{
    public class Vuelo : IValidable
    {
        public int Id { get; set; }
        public string Codigo { get; set; }

        public int AeropuertoOrigenId { get; set; }
        public Aeropuerto AeropuertoOrigen { get; set; }

        public int AeropuertoDestinoId { get; set; }
        public Aeropuerto AeropuertoDestino { get; set; }

        public DateTime FechaSalida { get; set; }
        public DateTime FechaLlegadaEstimada { get; set; }
        public decimal PrecioBase { get; set; }
        public int CapacidadAsientos { get; set; }
        public int AsientosDisponibles { get; set; }
        public EstadoVuelo Estado { get; set; }
        public bool Eliminado { get; set; }

        public Vuelo() { }

        public Vuelo(string codigo, int aeropuertoOrigenId, int aeropuertoDestinoId, DateTime fechaSalida,
                     DateTime fechaLlegadaEstimada, decimal precioBase, int capacidadAsientos)
        {
            Codigo = codigo;
            AeropuertoOrigenId = aeropuertoOrigenId;
            AeropuertoDestinoId = aeropuertoDestinoId;
            FechaSalida = fechaSalida;
            FechaLlegadaEstimada = fechaLlegadaEstimada;
            PrecioBase = precioBase;
            CapacidadAsientos = capacidadAsientos;
            AsientosDisponibles = capacidadAsientos;
            Estado = EstadoVuelo.PROGRAMADO;
            Eliminado = false;
        }

        public void Validar()
        {
            ValidarCodigo();

            if (AeropuertoOrigenId <= 0) throw new VueloException("El aeropuerto de origen es obligatorio");
            if (AeropuertoDestinoId <= 0) throw new VueloException("El aeropuerto de destino es obligatorio");
            if (AeropuertoOrigenId == AeropuertoDestinoId) throw new VueloException("El origen y el destino deben ser aeropuertos distintos");
            if (FechaSalida <= DateTime.Now) throw new VueloException("La fecha de salida debe ser futura");
            if (FechaLlegadaEstimada <= FechaSalida) throw new VueloException("La llegada estimada debe ser posterior a la salida");
            if (PrecioBase <= 0) throw new VueloException("El precio base debe ser mayor a cero");
            if (CapacidadAsientos <= 0) throw new VueloException("La capacidad de asientos debe ser mayor a cero");
            if (AsientosDisponibles < 0 || AsientosDisponibles > CapacidadAsientos) throw new VueloException("Los asientos disponibles deben estar entre 0 y la capacidad");
        }

        private void ValidarCodigo()
        {
            if (string.IsNullOrWhiteSpace(Codigo)) throw new VueloException("El código del vuelo es obligatorio");
            if (!Regex.IsMatch(Codigo, @"^[A-Z]{2}\d{1,4}$")) throw new VueloException("El código debe ser 2 letras mayúsculas seguidas de 1 a 4 números (ej. AS101)");
        }

        // --- Máquina de estados (LETRA 3.3) ---

        public bool EsTransicionValida(EstadoVuelo nuevo)
        {
            switch (Estado)
            {
                case EstadoVuelo.PROGRAMADO:
                    return nuevo == EstadoVuelo.DEMORADO || nuevo == EstadoVuelo.EMBARQUE || nuevo == EstadoVuelo.CANCELADO;
                case EstadoVuelo.DEMORADO:
                    return nuevo == EstadoVuelo.PROGRAMADO || nuevo == EstadoVuelo.EMBARQUE || nuevo == EstadoVuelo.CANCELADO;
                case EstadoVuelo.EMBARQUE:
                    return nuevo == EstadoVuelo.EN_VUELO || nuevo == EstadoVuelo.CANCELADO;
                case EstadoVuelo.EN_VUELO:
                    return nuevo == EstadoVuelo.ATERRIZADO;
                default:
                    // ATERRIZADO y CANCELADO son estados finales
                    return false;
            }
        }

        public void CambiarEstado(EstadoVuelo nuevo)
        {
            if (!EsTransicionValida(nuevo))
                throw new VueloException($"Transición inválida: un vuelo {Estado} no puede pasar a {nuevo}");
            Estado = nuevo;
        }

        // --- Reglas de negocio asociadas a estados ---

        public bool PuedeModificarse()
        {
            return Estado == EstadoVuelo.PROGRAMADO;
        }

        public bool DisponibleParaVenta()
        {
            return !Eliminado
                && AsientosDisponibles > 0
                && (Estado == EstadoVuelo.PROGRAMADO || Estado == EstadoVuelo.DEMORADO);
        }

        public void DescontarAsiento()
        {
            if (AsientosDisponibles <= 0) throw new VueloException("No quedan asientos disponibles en el vuelo");
            AsientosDisponibles--;
        }

        public void ReponerAsiento()
        {
            if (AsientosDisponibles >= CapacidadAsientos) throw new VueloException("No hay asientos vendidos para reponer");
            AsientosDisponibles++;
        }
    }
}
