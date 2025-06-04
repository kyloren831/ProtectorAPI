using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtectorAPI.Models
{
    [Table("Sistemas", Schema = "PROTECTOR_APP")]
    public class Sistema
    {
        [Key]
        public int IdSistema { get; set; }
        public string Descripcion { get; set; }
        public string Url { get; set; }
        public string Estado { get; set; }
        public List<Pantalla> Pantallas { get; set; }
    }
}
