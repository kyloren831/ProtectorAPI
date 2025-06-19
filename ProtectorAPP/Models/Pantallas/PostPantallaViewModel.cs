namespace ProtectorAPP.Models.Pantallas
{
    public class PostPantallaViewModel
    {
        public int IdPantalla { get; set; }
        public int IdSistema { get; set; }
        public string NombreSistema { get; set; }
        public string Descripcion { get; set; }
        public char Estado { get; set; }
        public IFormFile? ImagenFile { get; set; }
    }
}
