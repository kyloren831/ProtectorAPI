using System.ComponentModel.DataAnnotations.Schema;

namespace ProtectorAPI.Models
{
    [Table("Usuarios_Permisos_Pantallas", Schema = "PROTECTOR_APP")]
    public class UsuarioPermisoPantalla
    {
        public int IdUsuario { get; set; }
        public int IdPermiso { get; set; }
        public int IdPantalla { get; set; }

        public Usuario Usuario { get; set; }
        public Permiso Permiso { get; set; }
        public Pantalla Pantalla { get; set; }
    }
}
