namespace ProtectorAPP.Models.Pantallas
{
    public class PantallaConPermisosDTO
    {
        public int IdPantalla { get; set; }
        public string DescripcionPantalla { get; set; }
        public List<PermisoDTO> Permisos { get; set; }
    }
}
