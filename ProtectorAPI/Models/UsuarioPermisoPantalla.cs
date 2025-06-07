using System.ComponentModel.DataAnnotations.Schema;

namespace ProtectorAPI.Models
{
    [Table("USUARIOS_PERMISOS_PANTALLAS", Schema = "PROTECTOR_APP")]
    public class UsuarioPermisoPantalla
    {
        [Column("IDUSUARIO")]
        public int IdUsuario { get; set; }
        [Column("IDPERMISO")]
        public int IdPermiso { get; set; }
        [Column("IDPANTALLA")]
        public int IdPantalla { get; set; }

        public Usuario Usuario { get; set; }
        public Permiso Permiso { get; set; }
        public Pantalla Pantalla { get; set; }
    }
}
