using System.Collections.Generic;
using Caso.Uno.Principal.Front.views.Modelos;

namespace Caso.Uno.Principal.Front.views.Datos.Repositorios
{
    public interface IPrestamoRepositorio : IRepositorio<Prestamo>
    {
        IEnumerable<Prestamo> BuscarConDetalles(string texto);

        Prestamo ObtenerConDetalles(int id);

        bool EquipoTienePrestamoActivo(int equipoId);

        bool EquipoTienePrestamos(int equipoId);

        bool EmpleadoTienePrestamos(int empleadoId);
    }
}
