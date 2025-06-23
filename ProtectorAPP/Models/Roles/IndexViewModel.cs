using ProtectorAPP.Models.Pantallas;

namespace ProtectorAPP.Models.Roles
{
    public class IndexViewModel
    {
        public int IdRol { get; set; }
        public List<RolConPermisosViewModel>? RolConPermisos { get; set; }
        public List<PantallaViewModel> Pantallas { get; set; }
        public List<PermisoDTO> Permisos { get; set; } 
    }
}
