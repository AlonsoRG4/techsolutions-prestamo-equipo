using System.Collections.Generic;
using Caso.Uno.Principal.Front.views.Modelos;

namespace Caso.Uno.Principal.Front.views.Datos.Repositorios
{
    public interface IEquipoRepositorio : IRepositorio<Equipo>
    {
        IEnumerable<Equipo> Buscar(string texto);

        IEnumerable<Equipo> ObtenerDisponibles();
    }
}
