using System.Linq;
using System.Threading.Tasks;
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

        public async Task<ActionResult> Index()
        {
            var usuarios = UserManager.Users.ToList();
            var roles = new System.Collections.Generic.List<RolListaViewModel>();

            foreach (var rol in RoleManager.Roles.OrderBy(r => r.Name).ToList())
            {
                var total = 0;
                foreach (var usuario in usuarios)
                {
                    if (await UserManager.IsInRoleAsync(usuario.Id, rol.Name))
                    {
                        total++;
                    }
                }

                roles.Add(new RolListaViewModel { Id = rol.Id, Name = rol.Name, TotalUsuarios = total });
            }

            return View(roles);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                ModelState.AddModelError("", "El nombre del rol es obligatorio.");
                return View();
            }

            if (await RoleManager.RoleExistsAsync(nombre))
            {
                ModelState.AddModelError("", "Ya existe un rol con ese nombre.");
                return View();
            }

            await RoleManager.CreateAsync(new IdentityRole(nombre));
            TempData["MensajeExito"] = "Rol creado correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var rol = await RoleManager.FindByIdAsync(id);
            if (rol == null) return HttpNotFound();

            foreach (var usuario in UserManager.Users.ToList())
            {
                if (await UserManager.IsInRoleAsync(usuario.Id, rol.Name))
                {
                    TempData["MensajeError"] = "No se puede eliminar: hay usuarios con este rol asignado.";
                    return RedirectToAction("Index");
                }
            }

            await RoleManager.DeleteAsync(rol);
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
