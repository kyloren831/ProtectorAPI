using System.Text.Json.Serialization;

namespace ProtectorAPP.Models.Usuarios
{
    public class PutUsuarioViewModel
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string FotoUrl { get; set; }
        [JsonIgnore] 
        public IFormFile? ImagenFile { get; set; }
    }
}
