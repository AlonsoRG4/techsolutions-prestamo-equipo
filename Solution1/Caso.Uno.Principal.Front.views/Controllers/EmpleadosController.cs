using System;
using System.Net;
using System.Web.Mvc;
using Caso.Uno.Principal.Front.views.Datos;
using Caso.Uno.Principal.Front.views.Datos.Repositorios;
using Caso.Uno.Principal.Front.views.Modelos;
using Caso.Uno.Principal.Front.views.Servicios;

namespace Caso.Uno.Principal.Front.views.Controllers
{
    // ============================================================================
    // EmpleadosController
    // ----------------------------------------------------------------------------
    // CRUD completo del catálogo de Empleados (Nombre, Departamento, Correo,
    // Teléfono): son las personas a quienes se les puede prestar un equipo.
    // Igual que EquiposController, es exclusivo de Administrador y delega la
    // lógica de negocio en EmpleadoServicio (capa de Servicios).
    // ============================================================================
    [Authorize(Roles = "Administrador")]
    public class EmpleadosController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();
        private readonly EmpleadoServicio _servicio;

        public EmpleadosController()
        {
            _servicio = new EmpleadoServicio(new EmpleadoRepositorio(db), new PrestamoRepositorio(db));
        }

        // GET: Empleados/Index?buscar=texto — lista con buscador.
        public ActionResult Index(string buscar)
        {
            ViewBag.Buscar = buscar;
            return View(_servicio.Buscar(buscar));
        }

        // GET: Empleados/Details/5 — detalle de un empleado.
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var empleado = _servicio.ObtenerPorId(id.Value);
            if (empleado == null) return HttpNotFound();
            return View(empleado);
        }

        // GET: Empleados/Create — formulario de alta.
        public ActionResult Create()
        {
            return View(new Empleado());
        }

        // POST: Empleados/Create — guarda el empleado nuevo.
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

        // GET: Empleados/Edit/5 — formulario de edición.
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var empleado = _servicio.ObtenerPorId(id.Value);
            if (empleado == null) return HttpNotFound();
            return View(empleado);
        }

        // POST: Empleados/Edit/5 — guarda los cambios.
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

        // GET: Empleados/Delete/5 — confirmación antes de eliminar.
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var empleado = _servicio.ObtenerPorId(id.Value);
            if (empleado == null) return HttpNotFound();
            return View(empleado);
        }

        // POST: Empleados/Delete/5 — elimina, salvo que tenga préstamos
        // registrados (EmpleadoServicio.Eliminar lanza la excepción en ese caso).
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
