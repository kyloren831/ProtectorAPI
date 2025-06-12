using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ProtectorAPP.Models
{
    public class PostUsuario
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string FotoUrl { get; set; }

        [JsonIgnore]
        [NotMapped]
        public IFormFile? ImagenFile { get; set; }

    }
}
