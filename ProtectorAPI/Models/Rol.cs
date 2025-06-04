using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtectorAPI.Models
{
    [Table("Roles", Schema = "PROTECTOR_APP")]
    public class Rol
    {
        [Key]
        public int IdRol { get; set; }
        public string Descripcion { get; set; }
        public List<UsuarioRol> UsuarioRoles {  get; set; }

        public List <RolPermisoPantalla> RolPermisosPantallas {  get; set; }
    }
}
