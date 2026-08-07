using System.Linq;
using System.Web;
using System.Web.Mvc;
using Caso.Uno.Principal.Front.views.Datos;
using Caso.Uno.Principal.Front.views.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Caso.Uno.Principal.Front.views.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class RolesController : Controller
    {
        private ApplicationRoleManager _roleManager;
        private ApplicationUserManager _userManager;

        public ApplicationRoleManager RoleManager =>
            _roleManager ?? (_roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>());

        public ApplicationUserManager UserManager =>
            _userManager ?? (_userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());

        public ActionResult Index()
        {
            var roles = RoleManager.Roles
                .OrderBy(r => r.Name)
                .ToList()
                .Select(r => new RolListaViewModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    TotalUsuarios = UserManager.Users.ToList().Count(u => UserManager.IsInRole(u.Id, r.Name))
                })
                .ToList();

            return View(roles);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                ModelState.AddModelError("", "El nombre del rol es obligatorio.");
                return View();
            }

            if (RoleManager.RoleExists(nombre))
            {
                ModelState.AddModelError("", "Ya existe un rol con ese nombre.");
                return View();
            }

            RoleManager.Create(new IdentityRole(nombre));
            TempData["MensajeExito"] = "Rol creado correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var rol = RoleManager.FindById(id);
            if (rol == null) return HttpNotFound();

            var tieneUsuarios = UserManager.Users.ToList().Any(u => UserManager.IsInRole(u.Id, rol.Name));
            if (tieneUsuarios)
            {
                TempData["MensajeError"] = "No se puede eliminar: hay usuarios con este rol asignado.";
                return RedirectToAction("Index");
            }

            RoleManager.Delete(rol);
            TempData["MensajeExito"] = "Rol eliminado correctamente.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _roleManager?.Dispose();
                _userManager?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
