using System;
using System.Collections.Generic;
using Caso.Uno.Principal.Front.views.Datos.Repositorios;
using Caso.Uno.Principal.Front.views.Modelos;

namespace Caso.Uno.Principal.Front.views.Servicios
{
    /// <summary>Reglas de negocio del proceso de préstamo/devolución de equipo.</summary>
    public class PrestamoServicio
    {
        private readonly IPrestamoRepositorio _prestamoRepositorio;
        private readonly IEquipoRepositorio _equipoRepositorio;

        public PrestamoServicio(IPrestamoRepositorio prestamoRepositorio, IEquipoRepositorio equipoRepositorio)
        {
            _prestamoRepositorio = prestamoRepositorio;
            _equipoRepositorio = equipoRepositorio;
        }

        public IEnumerable<Prestamo> Buscar(string texto)
        {
            return _prestamoRepositorio.BuscarConDetalles(texto);
        }

        public Prestamo ObtenerConDetalles(int id)
        {
            return _prestamoRepositorio.ObtenerConDetalles(id);
        }

        /// <summary>Registra un préstamo nuevo. Lanza InvalidOperationException si el equipo ya está prestado.</summary>
        public void RegistrarPrestamo(Prestamo prestamo)
        {
            if (_prestamoRepositorio.EquipoTienePrestamoActivo(prestamo.EquipoId))
            {
                throw new InvalidOperationException("El equipo seleccionado ya se encuentra prestado.");
            }

            var equipo = _equipoRepositorio.ObtenerPorId(prestamo.EquipoId);
            if (equipo == null)
            {
                throw new InvalidOperationException("El equipo seleccionado no existe.");
            }

            prestamo.Estatus = EstatusPrestamo.Prestado;
            equipo.Estado = EstadoEquipo.Prestado;

            _prestamoRepositorio.Agregar(prestamo);
            _equipoRepositorio.Actualizar(equipo);
            _prestamoRepositorio.Guardar();
        }

        public void Actualizar(Prestamo prestamo)
        {
            _prestamoRepositorio.Actualizar(prestamo);
            _prestamoRepositorio.Guardar();
        }

        /// <summary>Marca el préstamo como devuelto y libera el equipo.</summary>
        public void RegistrarDevolucion(int prestamoId)
        {
            var prestamo = _prestamoRepositorio.ObtenerPorId(prestamoId);
            if (prestamo == null)
            {
                throw new InvalidOperationException("El préstamo no existe.");
            }

            if (prestamo.Estatus == EstatusPrestamo.Devuelto)
            {
                return;
            }

            prestamo.Estatus = EstatusPrestamo.Devuelto;
            prestamo.FechaEntrega = DateTime.Now;

            var equipo = _equipoRepositorio.ObtenerPorId(prestamo.EquipoId);
            if (equipo != null)
            {
                equipo.Estado = EstadoEquipo.Disponible;
                _equipoRepositorio.Actualizar(equipo);
            }

            _prestamoRepositorio.Actualizar(prestamo);
            _prestamoRepositorio.Guardar();
        }

        public void Eliminar(int id)
        {
            var prestamo = _prestamoRepositorio.ObtenerPorId(id);
            if (prestamo == null)
            {
                return;
            }

            if (prestamo.Estatus == EstatusPrestamo.Prestado)
            {
                var equipo = _equipoRepositorio.ObtenerPorId(prestamo.EquipoId);
                if (equipo != null)
                {
                    equipo.Estado = EstadoEquipo.Disponible;
                    _equipoRepositorio.Actualizar(equipo);
                }
            }

            _prestamoRepositorio.Eliminar(prestamo);
            _prestamoRepositorio.Guardar();
        }
    }
}
