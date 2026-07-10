using System.Collections.Generic;
using DTOs.DataTransferObjects.DTOsUsuario;
using DTOs.Mappers;
using LogicaAplicacion.ICasosDeUso.ICUUsuario;
using LogicaNegocio.Entidades;
using LogicaNegocio.IRepositorios;

namespace LogicaAplicacion.CasosDeUso.CUUsuario
{
    public class CUListarUsuarios : ICUListarUsuarios
    {
        private readonly IRepositorioUsuario _repoUsuario;

        public CUListarUsuarios(IRepositorioUsuario repoUsuario)
        {
            _repoUsuario = repoUsuario;
        }

        public List<DTOUsuario> Ejecutar()
        {
            List<Usuario> usuarios = _repoUsuario.GetAll();
            return MapperUsuario.ToListDtoUsuario(usuarios);
        }
    }
}
