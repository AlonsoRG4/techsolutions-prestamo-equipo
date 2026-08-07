using System.Collections.Generic;
using System.Linq;

namespace Caso.Uno.Principal.Front.views.Datos.Repositorios
{
    /// <summary>Operaciones CRUD genéricas de la capa de acceso a datos.</summary>
    public interface IRepositorio<T> where T : class
    {
        IQueryable<T> Consultar();

        IEnumerable<T> ObtenerTodos();

        T ObtenerPorId(int id);

        void Agregar(T entidad);

        void Actualizar(T entidad);

        void Eliminar(T entidad);

        void Guardar();
    }
}
