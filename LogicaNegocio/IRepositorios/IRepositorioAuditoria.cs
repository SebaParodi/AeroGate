using System.Collections.Generic;
using LogicaNegocio.Entidades;

namespace LogicaNegocio.IRepositorios
{
    public interface IRepositorioAuditoria
    {
        void Add(Auditoria a);                  // solo alta: la auditoría es inmutable
        Auditoria FindById(int id);
        List<Auditoria> GetAll();               // Administrador: todas las operaciones
        List<Auditoria> GetByUsuario(int usuarioId);  // Operador: solo las suyas
    }
}
