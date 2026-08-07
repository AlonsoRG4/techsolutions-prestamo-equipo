using System;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using Caso.Uno.Principal.Front.views.Datos;
using Caso.Uno.Principal.Front.views.Datos.Repositorios;
using Caso.Uno.Principal.Front.views.Modelos;
using Caso.Uno.Principal.Front.views.Servicios;

namespace Caso.Uno.Principal.Front.views.Controllers
{
    [Authorize(Roles = "Administrador,Operador")]
    public class EquiposController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private readonly EquipoServicio _servicio;

        public EquiposController()
        {
            _servicio = new EquipoServicio(new EquipoRepositorio(db), new PrestamoRepositorio(db));
        }

        public ActionResult Index(string buscar)
        {
            ViewBag.Buscar = buscar;
            return View(_servicio.Buscar(buscar));
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var equipo = _servicio.ObtenerPorId(id.Value);
            if (equipo == null) return HttpNotFound();
            return View(equipo);
        }

        public ActionResult Create()
        {
            ViewBag.Estados = EstadoEquipo.Todos;
            return View(new Equipo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Marca,Modelo,Serie,Estado")] Equipo equipo)
        {
            if (ModelState.IsValid)
            {
                _servicio.Registrar(equipo);
                TempData["MensajeExito"] = "Equipo registrado correctamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Estados = EstadoEquipo.Todos;
            return View(equipo);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var equipo = _servicio.ObtenerPorId(id.Value);
            if (equipo == null) return HttpNotFound();
            ViewBag.Estados = EstadoEquipo.Todos;
            return View(equipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Marca,Modelo,Serie,Estado")] Equipo equipo)
        {
            if (ModelState.IsValid)
            {
                _servicio.Actualizar(equipo);
                TempData["MensajeExito"] = "Equipo actualizado correctamente.";
                return RedirectToAction("Index");
            }

            ViewBag.Estados = EstadoEquipo.Todos;
            return View(equipo);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var equipo = _servicio.ObtenerPorId(id.Value);
            if (equipo == null) return HttpNotFound();
            return View(equipo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                _servicio.Eliminar(id);
                TempData["MensajeExito"] = "Equipo eliminado correctamente.";
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
