namespace ProtectorAPI.DTOs
{
    public class UsuarioDTO
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contrasenna { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string FotoUrl { get; set; }
        public char Estado { get; set; }
    }
}
