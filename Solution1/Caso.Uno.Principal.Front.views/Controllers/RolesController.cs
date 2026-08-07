using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Caso.Uno.Principal.Front.views.Datos;
using Caso.Uno.Principal.Front.views.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Caso.Uno.Principal.Front.views.Controllers
{
    /// <summary>
    /// El sistema trabaja con exactamente dos roles fijos (Administrador y
    /// Operador), creados por Database/Script_TechSolutionsDB.sql. Esta
    /// pantalla es de solo lectura: no se permite crear ni eliminar roles,
    /// solo consultar cuántos usuarios tiene cada uno.
    /// </summary>
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
