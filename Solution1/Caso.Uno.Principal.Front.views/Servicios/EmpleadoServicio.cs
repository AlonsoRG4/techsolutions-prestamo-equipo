using System.Collections.Generic;
using Caso.Uno.Principal.Front.views.Datos.Repositorios;
using Caso.Uno.Principal.Front.views.Modelos;

namespace Caso.Uno.Principal.Front.views.Servicios
{
    /// <summary>Reglas de negocio del catálogo de empleados.</summary>
    public class EmpleadoServicio
    {
        private readonly IEmpleadoRepositorio _empleadoRepositorio;
        private readonly IPrestamoRepositorio _prestamoRepositorio;

        public EmpleadoServicio(IEmpleadoRepositorio empleadoRepositorio, IPrestamoRepositorio prestamoRepositorio)
        {
            _empleadoRepositorio = empleadoRepositorio;
            _prestamoRepositorio = prestamoRepositorio;
        }

        public IEnumerable<Empleado> Buscar(string texto)
        {
            return _empleadoRepositorio.Buscar(texto);
        }

        public Empleado ObtenerPorId(int id)
        {
            return _empleadoRepositorio.ObtenerPorId(id);
        }

        public void Registrar(Empleado empleado)
        {
            _empleadoRepositorio.Agregar(empleado);
            _empleadoRepositorio.Guardar();
        }

        public void Actualizar(Empleado empleado)
        {
            _empleadoRepositorio.Actualizar(empleado);
            _empleadoRepositorio.Guardar();
        }

        /// <summary>Lanza InvalidOperationException si el empleado tiene préstamos registrados.</summary>
        public void Eliminar(int id)
        {
            if (_prestamoRepositorio.EmpleadoTienePrestamos(id))
            {
                throw new System.InvalidOperationException("No se puede eliminar: el empleado tiene préstamos registrados.");
            }

            var empleado = _empleadoRepositorio.ObtenerPorId(id);
            if (empleado == null)
            {
                return;
            }

            _empleadoRepositorio.Eliminar(empleado);
            _empleadoRepositorio.Guardar();
        }
    }
}
