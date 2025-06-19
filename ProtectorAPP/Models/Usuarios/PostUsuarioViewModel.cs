namespace ProtectorAPP.Models.Usuarios
{
    public class PostUsuarioViewModel
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public IFormFile? ImagenFile { get; set; }
    }
}
