using System.ComponentModel.DataAnnotations.Schema;

namespace ProtectorAPI.DTOs
{
    public class PermisoRolDTO
    {
        public int IdRol { get; set; }
        public int IdPermiso { get; set; }
        public int IdPantalla { get; set; }
    }
}
