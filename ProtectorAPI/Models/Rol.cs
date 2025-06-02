using System.ComponentModel.DataAnnotations;

namespace ProtectorAPI.Models
{
    public class Rol
    {
        [Key]
        public int IdRol { get; set; }
        public string Descripcion { get; set; }
        public char Estado { get; set; }
    }
}
