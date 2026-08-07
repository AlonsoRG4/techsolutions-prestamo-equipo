using System.Collections.Generic;
using Caso.Uno.Principal.Front.views.Modelos;

namespace Caso.Uno.Principal.Front.views.Datos.Repositorios
{
    public interface IEmpleadoRepositorio : IRepositorio<Empleado>
    {
        IEnumerable<Empleado> Buscar(string texto);
    }
}
