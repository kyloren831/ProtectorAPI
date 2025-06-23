namespace ProtectorAPP.Models.UsuariosPermisos
{
    public class UsuarioPermisoDTO
    {
        public int IdUsuario { get; set; }
        public int IdPantalla { get; set; }
        public int IdPermiso { get; set; }
        public string Sistema { get; set; }
        public string Pantalla { get; set; }
        public string Permiso { get; set; }
    }
}
