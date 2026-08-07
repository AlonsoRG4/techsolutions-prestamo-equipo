using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Caso.Uno.Principal.Front.views.Modelos;

namespace Caso.Uno.Principal.Front.views.Datos.Repositorios
{
    public class PrestamoRepositorio : RepositorioBase<Prestamo>, IPrestamoRepositorio
    {
        public PrestamoRepositorio(ApplicationDbContext contexto) : base(contexto)
        {
        }

        public IEnumerable<Prestamo> BuscarConDetalles(string texto)
        {
            var consulta = Consultar().Include(p => p.Equipo).Include(p => p.Empleado);

            if (!string.IsNullOrWhiteSpace(texto))
            {
                consulta = consulta.Where(p =>
                    p.Equipo.Nombre.Contains(texto) ||
                    p.Equipo.Marca.Contains(texto) ||
                    p.Empleado.Nombre.Contains(texto) ||
                    p.Estatus.Contains(texto));
            }

            return consulta.OrderByDescending(p => p.FechaPrestamo).ToList();
        }

        public Prestamo ObtenerConDetalles(int id)
        {
            return Consultar()
                .Include(p => p.Equipo)
                .Include(p => p.Empleado)
                .FirstOrDefault(p => p.Id == id);
        }

        public bool EquipoTienePrestamoActivo(int equipoId)
        {
            return Consultar().Any(p => p.EquipoId == equipoId && p.Estatus == EstatusPrestamo.Prestado);
        }

        public bool EquipoTienePrestamos(int equipoId)
        {
            return Consultar().Any(p => p.EquipoId == equipoId);
        }

        public bool EmpleadoTienePrestamos(int empleadoId)
        {
            return Consultar().Any(p => p.EmpleadoId == empleadoId);
        }
    }
}
