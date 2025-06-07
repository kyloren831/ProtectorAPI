using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtectorAPI.Models
{
    [Table("ROL_PERMISOS_PANTALLAS", Schema = "PROTECTOR_APP")]
    
    public class RolPermisoPantalla
    {
        
        [Column("IDROL")]
        public int IdRol { get; set; }

        [Column("IDPERMISO")]
        public int IdPermiso { get; set; }

        [Column("IDPANTALLA")]
        public int IdPantalla { get; set; }

        public Rol Rol { get; set; }
        public Permiso Permiso { get; set; }
        public Pantalla Pantalla { get; set; }
    }
}
