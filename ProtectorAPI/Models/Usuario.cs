using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtectorAPI.Models
{
    [Table("USUARIOS", Schema = "PROTECTOR_APP")]
    public class Usuario
    {
        [Column("IDUSUARIO")]
        [Key]
        public int IdUsuario { get; set; }
        [Column("NOMBRE")]
        public string Nombre { get; set; }

        [Column("CORREO")]
        public string Correo { get; set; }

        [Column("CONTRASENNA")]
        public string Contrasenna { get; set; }

        [Column("FECHACREACION")]
        public DateTime FechaCreacion { get; set; }

        [Column("FOTOURL")]
        public string FotoUrl { get; set; }

        [Column("ESTADO")]
        public char Estado { get; set; }
        public List<UsuarioRol>  UsuarioRoles { get; set; }
        public List<UsuarioPermisoPantalla> UsuarioPermisosPantallas { get; set; }
        public List <BitacoraUsuarios> BitacoraUsuarios { get; set; }
    }
}
