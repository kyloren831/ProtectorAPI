using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public List<RolPermisoPantalla> RolPermisosPantallas { get; set; }
        public List<UsuarioPermisoPantalla> UsuarioPermisosPantallas { get; set; }

    }
}
