using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Caso.Uno.Principal.Front.views.Datos
{
    /// <summary>Usuario de ASP.NET Identity. Los roles del sistema son Administrador y Operador.</summary>
    public class ApplicationUser : IdentityUser
    {
        public string NombreCompleto { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var identidadUsuario = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return identidadUsuario;
        }
    }
}
