using ProtectorAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProtectorAPI.DTOs
{
    public class RolConPermisosDTO
    {
        public int IdRol { get; set; }
        public int IdPantalla { get; set; }
        public int IdPermiso { get; set; }
        public string Rol { get; set; }
        public string Pantalla { get; set; }
        public string Permiso { get; set; }

    }
}
