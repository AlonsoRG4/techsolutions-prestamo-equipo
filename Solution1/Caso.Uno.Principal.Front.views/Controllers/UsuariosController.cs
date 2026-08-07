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
    // ============================================================================
    // UsuariosController
    // ----------------------------------------------------------------------------
    // Administración de las CUENTAS del sistema (tabla AspNetUsers), no confundir
    // con Empleados (que es un catálogo de negocio, no gente que inicia sesión).
    // Solo Administrador. Desde aquí se puede: ver todos los usuarios y sus
    // roles, cambiar los roles de un usuario, bloquear/desbloquear el acceso, y
    // eliminar una cuenta (menos la propia, para no auto-bloquearse el sistema).
    // Todos los métodos son async porque Microsoft.AspNet.Identity.Core no
    // expone versiones síncronas de estas operaciones (FindById, Delete,
    // GetRoles, etc. solo existen como FindByIdAsync, DeleteAsync, GetRolesAsync...).
    // ============================================================================
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public ApplicationUserManager UserManager =>
            _userManager ?? (_userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());

        public ApplicationRoleManager RoleManager =>
            _roleManager ?? (_roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>());

        // GET: Usuarios/Index?buscar=texto
        // Lista todos los usuarios con sus roles actuales y si están bloqueados.
        public async Task<ActionResult> Index(string buscar)
        {
            var usuarios = UserManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                usuarios = usuarios.Where(u =>
                    u.Email.Contains(buscar) ||
                    u.NombreCompleto.Contains(buscar));
            }

            var lista = new System.Collections.Generic.List<UsuarioListaViewModel>();

            foreach (var usuario in usuarios.OrderBy(u => u.Email).ToList())
            {
                var roles = await UserManager.GetRolesAsync(usuario.Id);

                lista.Add(new UsuarioListaViewModel
                {
                    Id = usuario.Id,
                    NombreCompleto = usuario.NombreCompleto,
                    Email = usuario.Email,
                    Bloqueado = usuario.LockoutEndDateUtc.HasValue && usuario.LockoutEndDateUtc.Value > DateTime.UtcNow,
                    Roles = roles.ToList()
                });
            }

            return View(lista);
        }

        // GET: Usuarios/EditarRoles/idDelUsuario
        // Muestra los 2 roles fijos (Administrador/Operador) con checkboxes,
        // marcando los que el usuario ya tiene asignados.
        public async Task<ActionResult> EditarRoles(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var usuario = await UserManager.FindByIdAsync(id);
            if (usuario == null) return HttpNotFound();

            var modelo = new EditarRolesViewModel
            {
                UsuarioId = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email,
                TodosLosRoles = RoleManager.Roles.Select(r => r.Name).OrderBy(r => r).ToList(),
                RolesSeleccionados = (await UserManager.GetRolesAsync(usuario.Id)).ToList()
            };

            return View(modelo);
        }

        // POST: Usuarios/EditarRoles
        // Quita todos los roles actuales y vuelve a asignar solo los que
        // llegaron marcados desde el formulario (rolesSeleccionados).
        // IMPORTANTE: el rol se "congela" en la cookie de sesión al iniciar
        // sesión, así que el usuario debe volver a iniciar sesión para que
        // el cambio de rol le tome efecto.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarRoles(string usuarioId, System.Collections.Generic.List<string> rolesSeleccionados)
        {
            var usuario = await UserManager.FindByIdAsync(usuarioId);
            if (usuario == null) return HttpNotFound();

            var rolesActuales = await UserManager.GetRolesAsync(usuarioId);
            rolesSeleccionados = rolesSeleccionados ?? new System.Collections.Generic.List<string>();

            if (rolesActuales.Any())
            {
                await UserManager.RemoveFromRolesAsync(usuarioId, rolesActuales.ToArray());
            }

            if (rolesSeleccionados.Any())
            {
                await UserManager.AddToRolesAsync(usuarioId, rolesSeleccionados.ToArray());
            }

            TempData["MensajeExito"] = "Roles actualizados correctamente.";
            return RedirectToAction("Index");
        }

        // POST: Usuarios/AlternarBloqueo/idDelUsuario
        // Bloquea o desbloquea el acceso del usuario poniendo (o quitando) una
        // fecha de bloqueo muy lejana en LockoutEndDateUtc. Un usuario bloqueado
        // no puede iniciar sesión aunque su contraseña sea correcta.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AlternarBloqueo(string id)
        {
            var usuario = await UserManager.FindByIdAsync(id);
            if (usuario == null) return HttpNotFound();

            var bloqueadoActualmente = usuario.LockoutEndDateUtc.HasValue && usuario.LockoutEndDateUtc.Value > DateTime.UtcNow;

            if (bloqueadoActualmente)
            {
                await UserManager.SetLockoutEndDateAsync(id, DateTimeOffset.UtcNow);
                TempData["MensajeExito"] = "Usuario desbloqueado.";
            }
            else
            {
                await UserManager.SetLockoutEndDateAsync(id, DateTimeOffset.UtcNow.AddYears(100));
                TempData["MensajeExito"] = "Usuario bloqueado.";
            }

            return RedirectToAction("Index");
        }

        // POST: Usuarios/Delete/idDelUsuario
        // Elimina la cuenta, salvo que sea la cuenta con la que se está
        // conectado en este momento (para no quedarse sin acceso al sistema).
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var usuario = await UserManager.FindByIdAsync(id);
            if (usuario == null) return HttpNotFound();

            if (usuario.UserName == User.Identity.Name)
            {
                TempData["MensajeError"] = "No puedes eliminar tu propia cuenta.";
                return RedirectToAction("Index");
            }

            await UserManager.DeleteAsync(usuario);
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
