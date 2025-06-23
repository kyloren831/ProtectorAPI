using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProtectorAPI.Models
{
    [Table("PERMISOS", Schema = "PROTECTOR_APP")]
    public class Permiso
    {
        [Key]
        [Column("IDPERMISO")]
        public int IdPermiso { get; set; }

        [Column("DESCRIPCION")]
        public string Descripcion { get; set; }
        [JsonIgnore]
        public List<RolPermisoPantalla> RolPermisosPantallas { get; set; }
        [JsonIgnore]
        public List<UsuarioPermisoPantalla> UsuarioPermisosPantallas { get; set; }

    }
}
