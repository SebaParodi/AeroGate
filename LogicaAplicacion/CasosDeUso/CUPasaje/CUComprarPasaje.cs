using DTOs.DataTransferObjects.DTOsPasaje;
using DTOs.Mappers;
using LogicaAplicacion.ICasosDeUso.ICUAuditoria;
using LogicaAplicacion.ICasosDeUso.ICUPasaje;
using LogicaNegocio.Entidades;
using LogicaNegocio.Enumerados;
using LogicaNegocio.Excepciones;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUPasaje
{
    public class CUComprarPasaje : ICUComprarPasaje
    {
        private readonly IRepositorioPasaje _repoPasaje;
        private readonly IRepositorioVuelo _repoVuelo;
        private readonly IRepositorioUsuario _repoUsuario;
        private readonly ICUGenerarAuditoria _cuAuditoria;

        public CUComprarPasaje(IRepositorioPasaje repoPasaje, IRepositorioVuelo repoVuelo,
                               IRepositorioUsuario repoUsuario, ICUGenerarAuditoria cuAuditoria)
        {
            _repoPasaje = repoPasaje;
            _repoVuelo = repoVuelo;
            _repoUsuario = repoUsuario;
            _cuAuditoria = cuAuditoria;
        }

        public DTOPasaje Ejecutar(DTOComprarPasaje dto)
        {
            // 1. El comprador debe ser un cliente
            Cliente cliente = _repoUsuario.FindById(dto.ClienteId) as Cliente;
            if (cliente == null)
                throw new UsuarioException("Solo los clientes pueden comprar pasajes");

            // 2. El vuelo debe estar en venta (PROGRAMADO o DEMORADO, con asientos, no eliminado)
            Vuelo vuelo = _repoVuelo.FindById(dto.VueloId);
            if (vuelo == null || vuelo.Eliminado)
                throw new VueloException($"No existe un vuelo con ID {dto.VueloId}");
            if (!vuelo.DisponibleParaVenta())
                throw new PasajeException($"El vuelo {vuelo.Codigo} no está disponible para la venta");

            // 3. Precio final con el descuento de la categoría vigente (foto histórica)
            decimal precioFinal = cliente.CalcularPrecioFinal(vuelo.PrecioBase);
            Pasaje pasaje = new Pasaje(cliente.Id, vuelo.Id, precioFinal, cliente.ObtenerDescuento());
            pasaje.Validar();

            // 4. Control de asientos + persistencia
            vuelo.DescontarAsiento();
            _repoPasaje.Add(pasaje);
            _repoVuelo.Update(vuelo);

            // 5. Recalcular la categoría del cliente con la compra ya incluida (LETRA 3.1)
            int compras = _repoPasaje.ContarComprasUltimos12Meses(cliente.Id);
            cliente.RecalcularCategoria(compras);
            _repoUsuario.Update(cliente);

            // 6. Auditoría automática (RF06)
            _cuAuditoria.Ejecutar(cliente.Id, TipoOperacion.COMPRA_PASAJE,
                $"Compra de pasaje del vuelo {vuelo.Codigo} por ${precioFinal}",
                vueloId: vuelo.Id, pasajeId: pasaje.Id);

            // Navegaciones en memoria para devolver el DTO completo
            pasaje.Cliente = cliente;
            pasaje.Vuelo = vuelo;
            return MapperPasaje.ToDtoPasaje(pasaje);
        }
    }
}
