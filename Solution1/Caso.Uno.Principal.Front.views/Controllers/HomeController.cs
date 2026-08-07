using System.Linq;
using System.Web.Mvc;
using Caso.Uno.Principal.Front.views.Datos;
using Caso.Uno.Principal.Front.views.Modelos;
using Caso.Uno.Principal.Front.views.ViewModels;

namespace Caso.Uno.Principal.Front.views.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

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
