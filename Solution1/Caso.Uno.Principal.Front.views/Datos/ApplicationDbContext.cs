using System.Data.Entity;
using Caso.Uno.Principal.Front.views.Modelos;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Caso.Uno.Principal.Front.views.Datos
{
    /// <summary>
    /// Contexto de Entity Framework 6. Incluye las tablas de ASP.NET Identity
    /// (AspNetUsers, AspNetRoles, AspNetUserRoles, etc.) más las tablas propias
    /// del sistema de préstamo de equipo. El esquema completo vive en
    /// Database/Script_TechSolutionsDB.sql y es autoritativo: este contexto no
    /// crea ni modifica la base de datos por su cuenta.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Equipo> Equipos { get; set; }

        public DbSet<Empleado> Empleados { get; set; }

        public DbSet<Prestamo> Prestamos { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Prestamo>()
                .HasRequired(p => p.Equipo)
                .WithMany(e => e.Prestamos)
                .HasForeignKey(p => p.EquipoId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Prestamo>()
                .HasRequired(p => p.Empleado)
                .WithMany(e => e.Prestamos)
                .HasForeignKey(p => p.EmpleadoId)
                .WillCascadeOnDelete(false);
        }
    }
}
