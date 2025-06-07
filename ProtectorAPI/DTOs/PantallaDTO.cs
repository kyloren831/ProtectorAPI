namespace ProtectorAPI.DTOs
{
    public class PantallaDTO
    {
        public int IdPantalla { get; set; }
        public int IdSistema { get; set; }
        public string Descripcion { get; set; }
        public string FotoUrl { get; set; }
        public char Estado { get; set; }
    }
}
