using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public Rol Rol { get; set; }
        [JsonIgnore]
        public Permiso Permiso { get; set; }
        [JsonIgnore]
        public Pantalla Pantalla { get; set; }
    }
}
