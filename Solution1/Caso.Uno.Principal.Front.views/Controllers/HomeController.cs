using System.Linq;
using System.Web.Mvc;
using Caso.Uno.Principal.Front.views.Datos;
using Caso.Uno.Principal.Front.views.Modelos;
using Caso.Uno.Principal.Front.views.ViewModels;

namespace Caso.Uno.Principal.Front.views.Controllers
{
    // ============================================================================
    // HomeController
    // ----------------------------------------------------------------------------
    // Muestra el Dashboard: la primera pantalla que ve cualquier usuario ya
    // autenticado (Administrador u Operador, [Authorize] sin rol específico).
    // Solo lee estadísticas (conteos) directo de la base de datos para pintar
    // las tarjetas del panel; no tiene CRUD propio.
    // ============================================================================
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Home/Index
        // Cuenta equipos (totales/disponibles/prestados), empleados y préstamos
        // (activos/devueltos) y los manda a la vista como PanelInicioViewModel.
        public ActionResult Index()
        {
            var modelo = new PanelInicioViewModel
            {
                TotalEquipos = db.Equipos.Count(),
                EquiposDisponibles = db.Equipos.Count(e => e.Estado == EstadoEquipo.Disponible),
                EquiposPrestados = db.Equipos.Count(e => e.Estado == EstadoEquipo.Prestado),
                TotalEmpleados = db.Empleados.Count(),
                PrestamosActivos = db.Prestamos.Count(p => p.Estatus == EstatusPrestamo.Prestado),
                PrestamosDevueltos = db.Prestamos.Count(p => p.Estatus == EstatusPrestamo.Devuelto)
            };

            return View(modelo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
