using System.ComponentModel.DataAnnotations;

namespace ProtectorAPI.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Contrasenia { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string FotoUrl { get; set; }
        public char Estado { get; set; }
    }
}
