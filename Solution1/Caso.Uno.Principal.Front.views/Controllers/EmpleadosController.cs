using System;
using System.Net;
using System.Web.Mvc;
using Caso.Uno.Principal.Front.views.Datos;
using Caso.Uno.Principal.Front.views.Datos.Repositorios;
using Caso.Uno.Principal.Front.views.Modelos;
using Caso.Uno.Principal.Front.views.Servicios;

namespace Caso.Uno.Principal.Front.views.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EmpleadosController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private readonly EmpleadoServicio _servicio;

        public EmpleadosController()
        {
            _servicio = new EmpleadoServicio(new EmpleadoRepositorio(db), new PrestamoRepositorio(db));
        }

        public ActionResult Index(string buscar)
        {
            ViewBag.Buscar = buscar;
            return View(_servicio.Buscar(buscar));
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var empleado = _servicio.ObtenerPorId(id.Value);
            if (empleado == null) return HttpNotFound();
            return View(empleado);
        }

        public ActionResult Create()
        {
            return View(new Empleado());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Departamento,Correo,Telefono")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                _servicio.Registrar(empleado);
                TempData["MensajeExito"] = "Empleado registrado correctamente.";
                return RedirectToAction("Index");
            }

            return View(empleado);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var empleado = _servicio.ObtenerPorId(id.Value);
            if (empleado == null) return HttpNotFound();
            return View(empleado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Departamento,Correo,Telefono")] Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                _servicio.Actualizar(empleado);
                TempData["MensajeExito"] = "Empleado actualizado correctamente.";
                return RedirectToAction("Index");
            }

            return View(empleado);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var empleado = _servicio.ObtenerPorId(id.Value);
            if (empleado == null) return HttpNotFound();
            return View(empleado);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _servicio.Eliminar(id);
                TempData["MensajeExito"] = "Empleado eliminado correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["MensajeError"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
