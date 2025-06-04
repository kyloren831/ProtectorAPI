using System.ComponentModel.DataAnnotations.Schema;

namespace ProtectorAPI.Models
{
    [Table("Rol_Permisos_Pantallas", Schema = "PROTECTOR_APP")]
    
    public class RolPermisoPantalla
    {
        public int IdRol { get; set; }
        public int IdPermiso { get; set; }
        public int IdPantalla { get; set; }
        public Rol Rol { get; set; }
        public Permiso Permiso { get; set; }
        public Pantalla Pantalla { get; set; }
    }
}
