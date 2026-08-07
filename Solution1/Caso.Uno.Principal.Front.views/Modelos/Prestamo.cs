using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caso.Uno.Principal.Front.views.Modelos
{
    /// <summary>Valores válidos para Prestamo.Estatus.</summary>
    public static class EstatusPrestamo
    {
        public const string Prestado = "Prestado";
        public const string Devuelto = "Devuelto";

        public static readonly string[] Todos = { Prestado, Devuelto };
    }

    [Table("Prestamos")]
    public class Prestamo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Selecciona un equipo")]
        [Display(Name = "Equipo")]
        public int EquipoId { get; set; }

        public virtual Equipo Equipo { get; set; }

        [Required(ErrorMessage = "Selecciona un empleado")]
        [Display(Name = "Empleado")]
        public int EmpleadoId { get; set; }

        public virtual Empleado Empleado { get; set; }

        [Required(ErrorMessage = "La fecha de préstamo es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Préstamo")]
        public DateTime FechaPrestamo { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Entrega")]
        public DateTime? FechaEntrega { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Estatus")]
        public string Estatus { get; set; } = EstatusPrestamo.Prestado;
    }
}
