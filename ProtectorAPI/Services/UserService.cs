using ProtectorAPI.Data;
using ProtectorAPI.DTOs;
using Microsoft.EntityFrameworkCore;
namespace ProtectorAPI.Services
{
    public interface IUserService
    {
        Task<List<PantallaConPermisosDTO>> ObtenerPantallasConPermisos(int idUsuario);
    }

    public class UserService : IUserService
    {
        private readonly ProtectorDbContext context;

        public UserService(ProtectorDbContext context)
        {
            this.context = context;
        }
        public async Task<List<PantallaConPermisosDTO>> ObtenerPantallasConPermisos(int idUsuario)
        {
            var usuario = await context.Usuarios
                .Include(u => u.UsuarioPermisosPantallas)
                    .ThenInclude(up => up.Permiso)
                .Include(u => u.UsuarioPermisosPantallas)
                    .ThenInclude(up => up.Pantalla)
                        .ThenInclude(p => p.Sistema)
                .Include(u => u.UsuarioRoles)
                    .ThenInclude(ur => ur.Rol)
                        .ThenInclude(r => r.RolPermisosPantallas)
                            .ThenInclude(rp => rp.Permiso)
                .Include(u => u.UsuarioRoles)
                    .ThenInclude(ur => ur.Rol)
                        .ThenInclude(r => r.RolPermisosPantallas)
                            .ThenInclude(rp => rp.Pantalla)
                                .ThenInclude(p => p.Sistema)
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

            if (usuario == null)
                return new List<PantallaConPermisosDTO>();

            // Permisos directos filtrados por pantallas activas y sistemas activos
            var directosPorPantalla = usuario.UsuarioPermisosPantallas
                .Where(up => up.Pantalla.Estado == 'A' && up.Pantalla.Sistema.Estado.Equals( 'A'))
                .GroupBy(up => up.Pantalla)
                .Select(g => new PantallaConPermisosDTO
                {
                    IdPantalla = g.Key.IdPantalla,
                    DescripcionPantalla = g.Key.Descripcion,
                    Permisos = g.Select(up => new PermisoDTO
                    {
                        IdPermiso = up.Permiso.IdPermiso,
                        Descripcion = up.Permiso.Descripcion
                    }).Distinct().ToList()
                });

            // Permisos por roles filtrados por pantallas activas y sistemas activos
            var permisosPorRoles = usuario.UsuarioRoles
                .SelectMany(ur => ur.Rol.RolPermisosPantallas)
                .Where(rp => rp.Pantalla.Estado == 'A' && rp.Pantalla.Sistema.Estado.Equals('A'))
                .GroupBy(rp => rp.Pantalla)
                .Select(g => new PantallaConPermisosDTO
                {
                    IdPantalla = g.Key.IdPantalla,
                    DescripcionPantalla = g.Key.Descripcion,
                    Permisos = g.Select(rp => new PermisoDTO
                    {
                        IdPermiso = rp.Permiso.IdPermiso,
                        Descripcion = rp.Permiso.Descripcion
                    }).Distinct().ToList()
                });

            // Combina ambos y elimina duplicados
            var pantallasTotales = directosPorPantalla
                .Concat(permisosPorRoles)
                .GroupBy(p => p.IdPantalla)
                .Select(g => new PantallaConPermisosDTO
                {
                    IdPantalla = g.Key,
                    DescripcionPantalla = g.First().DescripcionPantalla,
                    Permisos = g.SelectMany(p => p.Permisos)
                        .Distinct()
                        .ToList()
                })
                .ToList();

            return pantallasTotales;
        }
    }
}
