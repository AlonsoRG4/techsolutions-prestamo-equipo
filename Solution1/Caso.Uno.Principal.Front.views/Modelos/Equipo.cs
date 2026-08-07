using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caso.Uno.Principal.Front.views.Modelos
{
    /// <summary>Valores válidos para Equipo.Estado.</summary>
    public static class EstadoEquipo
    {
        public const string Disponible = "Disponible";
        public const string Prestado = "Prestado";
        public const string Mantenimiento = "Mantenimiento";

        public static readonly string[] Todos = { Disponible, Prestado, Mantenimiento };
    }

    [Table("Equipos")]
    public class Equipo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La marca es obligatoria")]
        [StringLength(80)]
        [Display(Name = "Marca")]
        public string Marca { get; set; }

        [Required(ErrorMessage = "El modelo es obligatorio")]
        [StringLength(80)]
        [Display(Name = "Modelo")]
        public string Modelo { get; set; }

        [Required(ErrorMessage = "El número de serie es obligatorio")]
        [StringLength(80)]
        [Display(Name = "Número de Serie")]
        public string Serie { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = EstadoEquipo.Disponible;

        public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    }
}
