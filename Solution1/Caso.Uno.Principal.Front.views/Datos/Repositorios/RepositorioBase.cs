using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Caso.Uno.Principal.Front.views.Datos.Repositorios
{
    public class RepositorioBase<T> : IRepositorio<T> where T : class
    {
        protected readonly ApplicationDbContext Contexto;
        protected readonly DbSet<T> ConjuntoDatos;

        public RepositorioBase(ApplicationDbContext contexto)
        {
            Contexto = contexto;
            ConjuntoDatos = contexto.Set<T>();
        }

        public IQueryable<T> Consultar()
        {
            return ConjuntoDatos.AsQueryable();
        }

        public IEnumerable<T> ObtenerTodos()
        {
            return ConjuntoDatos.ToList();
        }

        public T ObtenerPorId(int id)
        {
            return ConjuntoDatos.Find(id);
        }

        public void Agregar(T entidad)
        {
            ConjuntoDatos.Add(entidad);
        }

        public void Actualizar(T entidad)
        {
            Contexto.Entry(entidad).State = EntityState.Modified;
        }

        public void Eliminar(T entidad)
        {
            ConjuntoDatos.Remove(entidad);
        }

        public void Guardar()
        {
            Contexto.SaveChanges();
        }
    }
}
