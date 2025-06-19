using ProtectorAPI.Data;
using ProtectorAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
namespace ProtectorAPI.Services
{
    public interface IUserService
    {
        Task<List<PantallaConPermisosDTO>> ObtenerPantallasConPermisos(int idUsuario);
        Task<List<PantallaConPermisosDTO>> ObtenerPantallasConTodosLosPermisos();
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
            .Where(up => up.Pantalla.Estado.ToString().Trim() == "A" && up.Pantalla.Sistema.Estado.ToString().Trim() == "A")
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
                .Where(rp => rp.Pantalla.Estado.ToString().Trim() == "A" && rp.Pantalla.Sistema.Estado.ToString().Trim() == "A")
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


        public async Task<List<PantallaConPermisosDTO>> ObtenerPantallasConTodosLosPermisos()
        {
            // Traer las pantallas activas y sus relaciones
            var pantallas = await context.Pantallas
                .Where(p => p.Estado.ToString().Trim() == "A" && p.Sistema.Estado.ToString().Trim() == "A")
                .Include(p => p.RolPermisosPantallas)
                    .ThenInclude(rp => rp.Permiso)
                .Include(p => p.UsuarioPermisosPantallas)
                    .ThenInclude(up => up.Permiso)
                .Include(p => p.Sistema)
                .ToListAsync();

            var resultado = pantallas.Select(p => new PantallaConPermisosDTO
            {

                IdPantalla = p.IdPantalla,
                DescripcionPantalla = p.Descripcion,
                Permisos = p.RolPermisosPantallas
                        .Select(rp => new PermisoDTO
                        {
                            IdPermiso = rp.Permiso.IdPermiso,
                            Descripcion = rp.Permiso.Descripcion
                        })
                        .Concat(
                            p.UsuarioPermisosPantallas.Select(up => new PermisoDTO
                            {
                                IdPermiso = up.Permiso.IdPermiso,
                                Descripcion = up.Permiso.Descripcion
                            })
                        )
                        .GroupBy(p => new { p.IdPermiso, p.Descripcion }) // Agrupar por IdPermiso y Descripcion
                        .Select(g => g.First()) // Tomar el primer permiso en cada grupo
                        .ToList() 
            }).ToList();

            var real = resultado.Where(x => x.Permisos.Count > 0).ToList();

            return real;
        }


    }
}
