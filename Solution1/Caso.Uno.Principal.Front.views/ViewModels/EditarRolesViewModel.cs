using System.Collections.Generic;

namespace Caso.Uno.Principal.Front.views.ViewModels
{
    public class EditarRolesViewModel
    {
        public string UsuarioId { get; set; }

        public string NombreCompleto { get; set; }

        public string Email { get; set; }

        public List<string> TodosLosRoles { get; set; } = new List<string>();

        public List<string> RolesAsignados { get; set; } = new List<string>();

        public List<string> RolesSeleccionados { get; set; } = new List<string>();
    }
}
