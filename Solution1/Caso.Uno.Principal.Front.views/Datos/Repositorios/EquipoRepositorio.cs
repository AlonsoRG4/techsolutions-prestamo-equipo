using System.Collections.Generic;
using System.Linq;
using Caso.Uno.Principal.Front.views.Modelos;

namespace Caso.Uno.Principal.Front.views.Datos.Repositorios
{
    public class EquipoRepositorio : RepositorioBase<Equipo>, IEquipoRepositorio
    {
        public EquipoRepositorio(ApplicationDbContext contexto) : base(contexto)
        {
        }

        public IEnumerable<Equipo> Buscar(string texto)
        {
            var consulta = Consultar();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                consulta = consulta.Where(e =>
                    e.Nombre.Contains(texto) ||
                    e.Marca.Contains(texto) ||
                    e.Modelo.Contains(texto) ||
                    e.Serie.Contains(texto) ||
                    e.Estado.Contains(texto));
            }

            return consulta.OrderBy(e => e.Nombre).ToList();
        }

        public IEnumerable<Equipo> ObtenerDisponibles()
        {
            return Consultar().Where(e => e.Estado == EstadoEquipo.Disponible).OrderBy(e => e.Nombre).ToList();
        }
    }
}
