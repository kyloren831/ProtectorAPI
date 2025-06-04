using System.ComponentModel.DataAnnotations.Schema;

namespace ProtectorAPI.Models
{
    [Table("Usuarios_Roles", Schema = "PROTECTOR_APP")]
    public class UsuarioRol
    {
        public int IdRol { get; set; }
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
        public Rol Rol { get; set; }
    }
}
