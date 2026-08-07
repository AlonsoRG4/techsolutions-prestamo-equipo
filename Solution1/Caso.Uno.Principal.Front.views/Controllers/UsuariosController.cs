using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Caso.Uno.Principal.Front.views.Datos;
using Caso.Uno.Principal.Front.views.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace Caso.Uno.Principal.Front.views.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public ApplicationUserManager UserManager =>
            _userManager ?? (_userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());

        public ApplicationRoleManager RoleManager =>
            _roleManager ?? (_roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>());

        public ActionResult Index(string buscar)
        {
            var usuarios = UserManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                usuarios = usuarios.Where(u =>
                    u.Email.Contains(buscar) ||
                    u.NombreCompleto.Contains(buscar));
            }

            var lista = usuarios
                .OrderBy(u => u.Email)
                .ToList()
                .Select(u => new UsuarioListaViewModel
                {
                    Id = u.Id,
                    NombreCompleto = u.NombreCompleto,
                    Email = u.Email,
                    Bloqueado = u.LockoutEndDateUtc.HasValue && u.LockoutEndDateUtc.Value > DateTime.UtcNow,
                    Roles = UserManager.GetRoles(u.Id).ToList()
                })
                .ToList();

            return View(lista);
        }

        public ActionResult EditarRoles(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var usuario = UserManager.FindById(id);
            if (usuario == null) return HttpNotFound();

            var modelo = new EditarRolesViewModel
            {
                UsuarioId = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email,
                TodosLosRoles = RoleManager.Roles.Select(r => r.Name).OrderBy(r => r).ToList(),
                RolesSeleccionados = UserManager.GetRoles(usuario.Id).ToList()
            };

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarRoles(string usuarioId, System.Collections.Generic.List<string> rolesSeleccionados)
        {
            var usuario = UserManager.FindById(usuarioId);
            if (usuario == null) return HttpNotFound();

            var rolesActuales = await UserManager.GetRolesAsync(usuarioId);
            rolesSeleccionados = rolesSeleccionados ?? new System.Collections.Generic.List<string>();

            await UserManager.RemoveFromRolesAsync(usuarioId, rolesActuales.ToArray());

            if (rolesSeleccionados.Any())
            {
                await UserManager.AddToRolesAsync(usuarioId, rolesSeleccionados.ToArray());
            }

            TempData["MensajeExito"] = "Roles actualizados correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AlternarBloqueo(string id)
        {
            var usuario = UserManager.FindById(id);
            if (usuario == null) return HttpNotFound();

            var bloqueadoActualmente = usuario.LockoutEndDateUtc.HasValue && usuario.LockoutEndDateUtc.Value > DateTime.UtcNow;

            if (bloqueadoActualmente)
            {
                UserManager.SetLockoutEndDate(id, DateTimeOffset.UtcNow);
                TempData["MensajeExito"] = "Usuario desbloqueado.";
            }
            else
            {
                UserManager.SetLockoutEndDate(id, DateTimeOffset.UtcNow.AddYears(100));
                TempData["MensajeExito"] = "Usuario bloqueado.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var usuario = UserManager.FindById(id);
            if (usuario == null) return HttpNotFound();

            if (usuario.UserName == User.Identity.Name)
            {
                TempData["MensajeError"] = "No puedes eliminar tu propia cuenta.";
                return RedirectToAction("Index");
            }

            UserManager.Delete(usuario);
            TempData["MensajeExito"] = "Usuario eliminado correctamente.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userManager?.Dispose();
                _roleManager?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
