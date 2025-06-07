using Microsoft.EntityFrameworkCore;
using ProtectorAPI.Models;

namespace ProtectorAPI.Data
{
    public class ProtectorDbContext : DbContext
    {
        public ProtectorDbContext(DbContextOptions<ProtectorDbContext> options)
       : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Sistema> Sistemas { get; set; }
        public DbSet<Pantalla> Pantallas { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<RolPermisoPantalla> RolPermisosPantallas { get; set; }
        public DbSet<UsuarioPermisoPantalla> UsuarioPermisosPantallas { get; set; }
        public DbSet<UsuarioRol> UsuarioRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Llave primaria compuesta de rol_permisos_pantallas
            modelBuilder.Entity<RolPermisoPantalla>()
                .HasKey(p => new { p.IdRol, p.IdPantalla, p.IdPermiso });
            //Llave primaria compuesta de usuario_permisos_pantallas
            modelBuilder.Entity<UsuarioPermisoPantalla>()
                .HasKey(up => new { up.IdUsuario, up.IdPantalla, up.IdPermiso });
            //Llave primaria compuesta de usuario_rol
            modelBuilder.Entity<UsuarioRol>()
                .HasKey(ur => new { ur.IdUsuario, ur.IdRol });

            //Integridad referencial
            modelBuilder.Entity<RolPermisoPantalla>()
                .HasOne(rp => rp.Rol)
                .WithMany(r => r.RolPermisosPantallas)
                .HasForeignKey(rp => rp.IdRol);
            modelBuilder.Entity<RolPermisoPantalla>()
                .HasOne(rp => rp.Permiso)
                .WithMany(r => r.RolPermisosPantallas)
                .HasForeignKey(rp => rp.IdPermiso);

            modelBuilder.Entity<RolPermisoPantalla>()
                .HasOne(rp => rp.Pantalla)
                .WithMany(p => p.RolPermisosPantallas)
                .HasForeignKey(rp => rp.IdPantalla);

            modelBuilder.Entity<UsuarioPermisoPantalla>()
                .HasOne(up=> up.Usuario)
                .WithMany(p=>p.UsuarioPermisosPantallas)
                .HasForeignKey(up=>up.IdUsuario);

            modelBuilder.Entity<UsuarioPermisoPantalla>()
                .HasOne(up => up.Permiso)
                .WithMany(p => p.UsuarioPermisosPantallas)
                .HasForeignKey(up => up.IdPermiso);

            modelBuilder.Entity<UsuarioPermisoPantalla>()
                .HasOne(up => up.Pantalla)
                .WithMany(p => p.UsuarioPermisosPantallas)
                .HasForeignKey(up => up.IdPantalla);

            modelBuilder.Entity<UsuarioRol>()
                .HasOne(ur => ur.Usuario)
                .WithMany(r => r.UsuarioRoles)
                .HasForeignKey(ur => ur.IdUsuario);

            modelBuilder.Entity<UsuarioRol>()
                .HasOne(ur => ur.Rol)
                .WithMany(r => r.UsuarioRoles)
                .HasForeignKey(ur => ur.IdRol);

           modelBuilder.Entity<Pantalla>()
        .HasOne(p => p.Sistema)
        .WithMany(s => s.Pantallas)
        .HasForeignKey(p => p.IdSistema);

            

            // Completa la FK de RolPermisoPantalla a Pantalla
            
        }

    }
}
