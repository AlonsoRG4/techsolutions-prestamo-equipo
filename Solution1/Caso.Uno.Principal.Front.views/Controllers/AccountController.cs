using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Caso.Uno.Principal.Front.views.Datos;
using Caso.Uno.Principal.Front.views.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Caso.Uno.Principal.Front.views.Controllers
{
    /// <summary>Inicio/cierre de sesión con ASP.NET Identity y alta de usuarios (solo Administrador).</summary>
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public ApplicationSignInManager SignInManager =>
            _signInManager ?? (_signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>());

        public ApplicationUserManager UserManager =>
            _userManager ?? (_userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>());

        public ApplicationRoleManager RoleManager =>
            _roleManager ?? (_roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>());

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel modelo, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var resultado = await SignInManager.PasswordSignInAsync(
                modelo.Email, modelo.Password, modelo.RememberMe, shouldLockout: true);

            switch (resultado)
            {
                case SignInStatus.Success:
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");

                case SignInStatus.LockedOut:
                    ModelState.AddModelError("", "Esta cuenta ha sido bloqueada temporalmente por múltiples intentos fallidos.");
                    return View(modelo);

                default:
                    ModelState.AddModelError("", "Correo o contraseña incorrectos.");
                    return View(modelo);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult Register()
        {
            ViewBag.Roles = RoleManager.Roles.Select(r => r.Name).OrderBy(r => r).ToList();
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel modelo)
        {
            ViewBag.Roles = RoleManager.Roles.Select(r => r.Name).OrderBy(r => r).ToList();

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = new ApplicationUser
            {
                UserName = modelo.Email,
                Email = modelo.Email,
                NombreCompleto = modelo.NombreCompleto
            };

            var resultado = await UserManager.CreateAsync(usuario, modelo.Password);

            if (resultado.Succeeded)
            {
                await UserManager.AddToRoleAsync(usuario.Id, modelo.Rol);
                TempData["MensajeExito"] = $"Usuario {modelo.Email} creado correctamente con el rol {modelo.Rol}.";
                return RedirectToAction("Index", "Usuarios");
            }

            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError("", error);
            }

            return View(modelo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userManager?.Dispose();
                _signInManager?.Dispose();
                _roleManager?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
