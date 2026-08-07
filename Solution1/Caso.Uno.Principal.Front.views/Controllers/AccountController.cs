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
    // ============================================================================
    // AccountController
    // ----------------------------------------------------------------------------
    // Controlador de autenticación del sistema, basado en ASP.NET Identity.
    // Se encarga de: iniciar sesión (Login), cerrar sesión (LogOff) y crear
    // nuevos usuarios asignándoles un rol (Register, solo para Administrador).
    // No tiene CRUD propio: usa los "managers" de Identity (UserManager,
    // SignInManager, RoleManager) que trabajan sobre las tablas AspNetUsers,
    // AspNetRoles y AspNetUserRoles creadas por el script SQL.
    // ============================================================================
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

        // GET: Account/Login
        // Muestra el formulario de inicio de sesión. [AllowAnonymous] porque
        // cualquiera (sin haber iniciado sesión) debe poder ver esta pantalla.
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        // POST: Account/Login
        // Valida usuario/contraseña contra AspNetUsers (PasswordSignInAsync) y,
        // si son correctos, crea la cookie de autenticación. Si venía de una
        // página protegida (returnUrl), regresa ahí; si no, va al Dashboard.
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

        // POST: Account/LogOff
        // Cierra la sesión actual (borra la cookie de autenticación de Identity).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        // GET: Account/Register
        // Formulario para crear un usuario nuevo. Solo lo puede abrir un
        // Administrador (los usuarios no se auto-registran en este sistema).
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public ActionResult Register()
        {
            ViewBag.Roles = RoleManager.Roles.Select(r => r.Name).OrderBy(r => r).ToList();
            return View(new RegisterViewModel());
        }

        // POST: Account/Register
        // Crea el usuario en AspNetUsers (CreateAsync) y le asigna el rol elegido
        // (AddToRoleAsync). Si la asignación de rol falla, se borra el usuario
        // recién creado para no dejar una cuenta "sin rol" que nunca podría
        // entrar a ningún módulo del sistema.
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
                var resultadoRol = await UserManager.AddToRoleAsync(usuario.Id, modelo.Rol);

                if (!resultadoRol.Succeeded)
                {
                    // El usuario ya se creó pero se quedó sin rol: sin rol no puede
                    // entrar a ningún módulo (se vería como un "bucle" de login).
                    // Se revierte la creación para no dejar cuentas huérfanas.
                    await UserManager.DeleteAsync(usuario);
                    foreach (var error in resultadoRol.Errors)
                    {
                        ModelState.AddModelError("", "No se pudo asignar el rol: " + error);
                    }
                    return View(modelo);
                }

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
