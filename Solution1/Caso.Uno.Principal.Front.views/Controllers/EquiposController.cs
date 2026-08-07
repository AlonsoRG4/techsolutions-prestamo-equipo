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
    // ============================================================================
    // EquiposController
    // ----------------------------------------------------------------------------
    // CRUD completo del catálogo de Equipos tecnológicos (Nombre, Marca, Modelo,
    // Serie, Estado). Solo lo puede usar el Administrador: el Operador no ve
    // este módulo (por eso el rol Operador no aparece en [Authorize]).
    // No habla directo con Entity Framework: delega en EquipoServicio (capa de
    // negocio), que a su vez usa EquipoRepositorio/PrestamoRepositorio (capa de
    // datos). Esa separación es la "arquitectura por capas" del proyecto.
    // ============================================================================
    [Authorize(Roles = "Administrador")]
    public class EquiposController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private readonly EquipoServicio _servicio;

        public EquiposController()
        {
            _servicio = new EquipoServicio(new EquipoRepositorio(db), new PrestamoRepositorio(db));
        }

        // GET: Equipos/Index?buscar=texto
        // Lista todos los equipos; si viene "buscar", filtra por nombre, marca,
        // modelo, serie o estado (ver EquipoRepositorio.Buscar).
        public ActionResult Index(string buscar)
        {
            ViewBag.Buscar = buscar;
            return View(_servicio.Buscar(buscar));
        }

        // GET: Equipos/Details/5 — muestra el detalle de un equipo.
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var equipo = _servicio.ObtenerPorId(id.Value);
            if (equipo == null) return HttpNotFound();
            return View(equipo);
        }

        // GET: Equipos/Create — formulario para registrar un equipo nuevo.
        public ActionResult Create()
        {
            ViewBag.Estados = EstadoEquipo.Todos;
            return View(new Equipo());
        }

        // POST: Equipos/Create — valida el modelo y guarda el equipo nuevo.
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

        // GET: Equipos/Edit/5 — formulario de edición con los datos actuales.
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var equipo = _servicio.ObtenerPorId(id.Value);
            if (equipo == null) return HttpNotFound();
            ViewBag.Estados = EstadoEquipo.Todos;
            return View(equipo);
        }

        // POST: Equipos/Edit/5 — guarda los cambios del equipo.
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

        // GET: Equipos/Delete/5 — pantalla de confirmación antes de eliminar.
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var equipo = _servicio.ObtenerPorId(id.Value);
            if (equipo == null) return HttpNotFound();
            return View(equipo);
        }

        // POST: Equipos/Delete/5 — elimina el equipo. EquipoServicio.Eliminar
        // lanza InvalidOperationException si el equipo tiene préstamos
        // registrados, para no romper la integridad referencial con Prestamos.
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
