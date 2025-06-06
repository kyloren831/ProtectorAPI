using System.ComponentModel.DataAnnotations.Schema;

namespace ProtectorAPI.Models
{
    [Table("USUARIOS_ROLES", Schema = "PROTECTOR_APP")]
    public class UsuarioRol
    {
        [Column("IDROL")]
        public int IdRol { get; set; }
        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
        public Rol Rol { get; set; }
    }
}
