using System.Collections.Generic;
using Caso.Uno.Principal.Front.views.Datos.Repositorios;
using Caso.Uno.Principal.Front.views.Modelos;

namespace Caso.Uno.Principal.Front.views.Servicios
{
    /// <summary>Reglas de negocio del catálogo de equipos.</summary>
    public class EquipoServicio
    {
        private readonly IEquipoRepositorio _equipoRepositorio;
        private readonly IPrestamoRepositorio _prestamoRepositorio;

        public EquipoServicio(IEquipoRepositorio equipoRepositorio, IPrestamoRepositorio prestamoRepositorio)
        {
            _equipoRepositorio = equipoRepositorio;
            _prestamoRepositorio = prestamoRepositorio;
        }

        public IEnumerable<Equipo> Buscar(string texto)
        {
            return _equipoRepositorio.Buscar(texto);
        }

        public Equipo ObtenerPorId(int id)
        {
            return _equipoRepositorio.ObtenerPorId(id);
        }

        public IEnumerable<Equipo> ObtenerDisponibles()
        {
            return _equipoRepositorio.ObtenerDisponibles();
        }

        public void Registrar(Equipo equipo)
        {
            _equipoRepositorio.Agregar(equipo);
            _equipoRepositorio.Guardar();
        }

        public void Actualizar(Equipo equipo)
        {
            _equipoRepositorio.Actualizar(equipo);
            _equipoRepositorio.Guardar();
        }

        /// <summary>Lanza InvalidOperationException si el equipo tiene préstamos registrados.</summary>
        public void Eliminar(int id)
        {
            if (_prestamoRepositorio.EquipoTienePrestamos(id))
            {
                throw new System.InvalidOperationException("No se puede eliminar: el equipo tiene préstamos registrados.");
            }

            var equipo = _equipoRepositorio.ObtenerPorId(id);
            if (equipo == null)
            {
                return;
            }

            _equipoRepositorio.Eliminar(equipo);
            _equipoRepositorio.Guardar();
        }
    }
}
