using System.Collections.Generic;
using System.Linq;
using LogicaNegocio.Enumerados;
using LogicaNegocio.Excepciones;
using LogicaNegocio.ValueObjects.VOUsuario;

namespace LogicaNegocio.Entidades
{
    public class Cliente : Usuario
    {
        // Reglas de categoría (LETRA 3.1): rangos de compras en los últimos 12 meses
        private const int ComprasMinimasFrecuente = 5;
        private const int ComprasMinimasPremium = 10;

        public string Documento { get; set; }
        public string Direccion { get; set; }
        public CategoriaCliente Categoria { get; set; }
        public List<Pasaje> Pasajes { get; set; } = new List<Pasaje>();

        public Cliente() { }

        public Cliente(NombreCompletoVO nombreCompleto, string email, string pass, string documento, string direccion)
            : base(nombreCompleto, email, pass)
        {
            Documento = documento;
            Direccion = direccion;
            Categoria = CategoriaCliente.ESTANDAR;
        }

        public void RecalcularCategoria(int comprasUlt12Meses)
        {
            if (comprasUlt12Meses < 0) throw new UsuarioException("La cantidad de compras no puede ser negativa");

            if (comprasUlt12Meses >= ComprasMinimasPremium) Categoria = CategoriaCliente.PREMIUM;
            else if (comprasUlt12Meses >= ComprasMinimasFrecuente) Categoria = CategoriaCliente.FRECUENTE;
            else Categoria = CategoriaCliente.ESTANDAR;
        }

        public decimal CalcularPrecioFinal(decimal precioBase)
        {
            if (precioBase <= 0) throw new UsuarioException("El precio base debe ser mayor a cero");
            return precioBase * (1 - ObtenerDescuento());
        }

        public decimal CalcularReintegro(decimal precioPagado)
        {
            if (precioPagado <= 0) throw new UsuarioException("El precio pagado debe ser mayor a cero");
            return precioPagado * ObtenerPorcentajeReintegro();
        }

        public decimal ObtenerDescuento()
        {
            switch (Categoria)
            {
                case CategoriaCliente.PREMIUM: return 0.15m;
                case CategoriaCliente.FRECUENTE: return 0.10m;
                default: return 0m;
            }
        }

        public decimal ObtenerPorcentajeReintegro()
        {
            switch (Categoria)
            {
                case CategoriaCliente.PREMIUM: return 1.00m;
                case CategoriaCliente.FRECUENTE: return 0.90m;
                default: return 0.70m;
            }
        }

        public override void Validar()
        {
            base.Validar();
            ValidarDocumento();
            if (string.IsNullOrWhiteSpace(Direccion)) throw new UsuarioException("La dirección es obligatoria");
        }

        private void ValidarDocumento()
        {
            if (string.IsNullOrWhiteSpace(Documento)) throw new UsuarioException("El documento es obligatorio");
            if (!Documento.All(char.IsDigit)) throw new UsuarioException("El documento debe contener solo dígitos");
            if (Documento.Length < 7 || Documento.Length > 8) throw new UsuarioException("El documento debe tener 7 u 8 dígitos");
        }
    }
}
