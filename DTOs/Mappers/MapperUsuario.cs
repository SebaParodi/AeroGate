using System.Collections.Generic;
using System.Linq;
using DTOs.DataTransferObjects.DTOsUsuario;
using LogicaNegocio.Entidades;

namespace DTOs.Mappers
{
    public class MapperUsuario
    {
        // El rol se deriva del tipo concreto de la jerarquía TPH
        public static string ObtenerRol(Usuario u)
        {
            if (u is Administrador) return "ADMINISTRADOR";
            if (u is Operador) return "OPERADOR";
            if (u is Cliente) return "CLIENTE";
            return "";
        }

        public static DTOUsuario ToDtoUsuario(Usuario u)
        {
            return new DTOUsuario
            {
                Id = u.Id,
                NombreCompleto = u.NombreCompleto != null ? u.NombreCompleto.ToString() : "",
                Email = u.Email,
                Rol = ObtenerRol(u),
                Categoria = u is Cliente cliente ? cliente.Categoria.ToString() : ""
            };
        }

        public static List<DTOUsuario> ToListDtoUsuario(List<Usuario> usuarios)
        {
            return usuarios.Select(u => ToDtoUsuario(u)).ToList();
        }

        public static DTOCliente ToDtoCliente(Cliente c)
        {
            return new DTOCliente
            {
                Id = c.Id,
                NombreCompleto = c.NombreCompleto != null ? c.NombreCompleto.ToString() : "",
                Email = c.Email,
                Documento = c.Documento,
                Direccion = c.Direccion,
                Categoria = c.Categoria.ToString()
            };
        }

        public static List<DTOCliente> ToListDtoCliente(List<Cliente> clientes)
        {
            return clientes.Select(c => ToDtoCliente(c)).ToList();
        }
    }
}
