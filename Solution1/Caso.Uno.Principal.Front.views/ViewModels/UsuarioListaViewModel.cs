using System.Collections.Generic;

namespace Caso.Uno.Principal.Front.views.ViewModels
{
    public class UsuarioListaViewModel
    {
        public string Id { get; set; }

        public string NombreCompleto { get; set; }

        public string Email { get; set; }

        public bool Bloqueado { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }
}
