using System;
using System.Net;
using System.Web.Mvc;
using Caso.Uno.Principal.Front.views.Datos;
using Caso.Uno.Principal.Front.views.Datos.Repositorios;
using Caso.Uno.Principal.Front.views.Modelos;
using Caso.Uno.Principal.Front.views.Servicios;

namespace Caso.Uno.Principal.Front.views.Controllers
{
    [Authorize(Roles = "Administrador,Operador")]
    public class PrestamosController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private readonly PrestamoServicio _servicio;
        private readonly IEquipoRepositorio _equipoRepositorio;
        private readonly IEmpleadoRepositorio _empleadoRepositorio;

        public PrestamosController()
        {
            _equipoRepositorio = new EquipoRepositorio(db);
            _empleadoRepositorio = new EmpleadoRepositorio(db);
            _servicio = new PrestamoServicio(new PrestamoRepositorio(db), _equipoRepositorio);
        }

        public ActionResult Index(string buscar)
        {
            ViewBag.Buscar = buscar;
            return View(_servicio.Buscar(buscar));
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var prestamo = _servicio.ObtenerConDetalles(id.Value);
            if (prestamo == null) return HttpNotFound();
            return View(prestamo);
        }

        public ActionResult Create()
        {
            CargarListasParaCrear();
            return View(new Prestamo { FechaPrestamo = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EquipoId,EmpleadoId,FechaPrestamo,FechaEntrega")] Prestamo prestamo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _servicio.RegistrarPrestamo(prestamo);
                    TempData["MensajeExito"] = "Préstamo registrado correctamente.";
                    return RedirectToAction("Index");
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            CargarListasParaCrear(prestamo);
            return View(prestamo);
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var prestamo = _equipoYPrestamo(id.Value);
            if (prestamo == null) return HttpNotFound();
            CargarListasParaEditar(prestamo);
            return View(prestamo);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EquipoId,EmpleadoId,FechaPrestamo,FechaEntrega")] Prestamo prestamo)
        {
            if (ModelState.IsValid)
            {
                // El Estatus solo cambia a través de la acción "Devolver" para mantener
                // sincronizado el estado del equipo; aquí se conserva el valor guardado.
                var actual = _servicio.ObtenerConDetalles(prestamo.Id);
                if (actual == null) return HttpNotFound();
                prestamo.Estatus = actual.Estatus;

                _servicio.Actualizar(prestamo);
                TempData["MensajeExito"] = "Préstamo actualizado correctamente.";
                return RedirectToAction("Index");
            }

            CargarListasParaEditar(prestamo);
            return View(prestamo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Devolver(int id)
        {
            try
            {
                _servicio.RegistrarDevolucion(id);
                TempData["MensajeExito"] = "Devolución registrada correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["MensajeError"] = ex.Message;
            }

            return RedirectToAction("Details", new { id });
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var prestamo = _servicio.ObtenerConDetalles(id.Value);
            if (prestamo == null) return HttpNotFound();
            return View(prestamo);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _servicio.Eliminar(id);
            TempData["MensajeExito"] = "Préstamo eliminado correctamente.";
            return RedirectToAction("Index");
        }

        private Prestamo _equipoYPrestamo(int id)
        {
            return _servicio.ObtenerConDetalles(id);
        }

        private void CargarListasParaCrear(Prestamo prestamo = null)
        {
            ViewBag.EquipoId = new SelectList(_equipoRepositorio.ObtenerDisponibles(), "Id", "Nombre", prestamo?.EquipoId);
            ViewBag.EmpleadoId = new SelectList(_empleadoRepositorio.ObtenerTodos(), "Id", "Nombre", prestamo?.EmpleadoId);
        }

        private void CargarListasParaEditar(Prestamo prestamo)
        {
            // En edición se incluye también el equipo ya asignado a este préstamo,
            // aunque su estado actual sea "Prestado".
            ViewBag.EquipoId = new SelectList(_equipoRepositorio.ObtenerTodos(), "Id", "Nombre", prestamo.EquipoId);
            ViewBag.EmpleadoId = new SelectList(_empleadoRepositorio.ObtenerTodos(), "Id", "Nombre", prestamo.EmpleadoId);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
