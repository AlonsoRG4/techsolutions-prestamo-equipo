using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using Caso.Uno.Principal.Front.views.Datos;

namespace Caso.Uno.Principal.Front.views
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // El esquema (incluidas las tablas de ASP.NET Identity) vive en
            // Database/Script_TechSolutionsDB.sql: EF nunca debe crear ni
            // alterar la base de datos por su cuenta.
            Database.SetInitializer<ApplicationDbContext>(null);
        }
    }
}
