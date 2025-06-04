using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtectorAPI.Models
{
    [Table("Permisos", Schema = "PROTECTOR_APP")]
    public class Permiso
    {
        [Key]
        public int IdPermiso { get; set; }
        public string Descripcion { get; set; }
        public List<RolPermisoPantalla> RolPermisosPantallas { get; set; }
        public List<UsuarioPermisoPantalla> UsuarioPermisosPantallas { get; set; }

    }
}
