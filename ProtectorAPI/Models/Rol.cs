using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtectorAPI.Models
{
    [Table("ROLES", Schema = "PROTECTOR_APP")]
    public class Rol
    {
        [Key]
        [Column("IDROL")]
        public int IdRol { get; set; }
        [Column("DESCRIPCION")]
        public string Descripcion { get; set; }
        public List<UsuarioRol> UsuarioRoles {  get; set; }

        public List <RolPermisoPantalla> RolPermisosPantallas {  get; set; }
    }
}
