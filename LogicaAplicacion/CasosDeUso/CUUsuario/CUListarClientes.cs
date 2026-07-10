using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsUsuario;
using DTOs.Mappers;
using LogicaAplicacion.ICasosDeUso.ICUUsuario;
using LogicaNegocio.Entidades;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUUsuario
{
    public class CUListarClientes : ICUListarClientes
    {
        private readonly IRepositorioUsuario _repoUsuario;

        public CUListarClientes(IRepositorioUsuario repoUsuario)
        {
            _repoUsuario = repoUsuario;
        }

        public List<DTOCliente> Ejecutar()
        {
            List<Cliente> clientes = _repoUsuario.GetClientes();
            return MapperUsuario.ToListDtoCliente(clientes);
        }
    }
}
