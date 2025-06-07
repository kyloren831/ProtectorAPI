using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtectorAPI.Models
{
    [Table("PANTALLAS", Schema = "PROTECTOR_APP")]
    public class Pantalla
    {
        [Key]

        [Column("IDPANTALLA")]
        public int IdPantalla { get; set; }

        [Column("IDSISTEMA")]
        public int IdSistema { get; set; }

        [Column("DESCRIPCION")]
        public string Descripcion { get; set; }

        [Column("FOTOURL")]
        public string FotoUrl { get; set; }

        [Column("ESTADO")]
        public char Estado { get; set; }
        public Sistema Sistema { get; set; }
        public List<RolPermisoPantalla> RolPermisosPantallas { get; set; }
        public List<UsuarioPermisoPantalla> UsuarioPermisosPantallas { get; set; }


    }
}
