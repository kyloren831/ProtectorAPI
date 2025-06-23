using ProtectorAPP.Models.Pantallas;
using ProtectorAPP.Models.Roles;
using ProtectorAPP.Models.Usuarios;

namespace ProtectorAPP.Models.UsuariosPermisos
{
    public class IndexViewModel
    {
        public UsuarioViewModel Usuario { get; set; }
        public List<GetUsuarioRolDTO> UsuarioRoles { get; set; }
        public List <UsuarioPermisoDTO> UsuariosPermisos { get; set; }
        public List<PantallaViewModel> Pantallas { get; set; }
        public List<PermisoDTO> Permisos { get; set; }
        public List<RolViewModel> Roles {  get; set; } 
    }
}
