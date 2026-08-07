using System.Collections.Generic;
using System.Linq;
using Caso.Uno.Principal.Front.views.Modelos;

namespace Caso.Uno.Principal.Front.views.Datos.Repositorios
{
    public class EmpleadoRepositorio : RepositorioBase<Empleado>, IEmpleadoRepositorio
    {
        public EmpleadoRepositorio(ApplicationDbContext contexto) : base(contexto)
        {
        }

        public IEnumerable<Empleado> Buscar(string texto)
        {
            var consulta = Consultar();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                consulta = consulta.Where(e =>
                    e.Nombre.Contains(texto) ||
                    e.Departamento.Contains(texto) ||
                    e.Correo.Contains(texto) ||
                    e.Telefono.Contains(texto));
            }

            return consulta.OrderBy(e => e.Nombre).ToList();
        }
    }
}
