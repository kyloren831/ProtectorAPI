using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtectorAPI.Models
{
    [Table("SISTEMAS", Schema = "PROTECTOR_APP")]
    public class Sistema
    {
        [Key]
        [Column("IDSISTEMA")]
        public int IdSistema { get; set; }

        [Column("DESCRIPCION")]
        public string Descripcion { get; set; }

        [Column("URL")]
        public string Url { get; set; }

        [Column("ESTADO")]
        public string Estado { get; set; }

        public List<Pantalla> Pantallas { get; set; }
    }

}
