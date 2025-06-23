using Microsoft.AspNetCore.Mvc;
using ProtectorAPI.DTOs;
using ProtectorAPI.Models;
using ProtectorAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ProtectorAPI.Controllers
{

    [Route("api/[controller]")]

    [ApiController]

    public class UsuarioPermisoPantallaController : ControllerBase
    {

        private readonly ProtectorDbContext _context;

        public UsuarioPermisoPantallaController(ProtectorDbContext context)
        {
            this._context = context;
        }

/////////////////////////////////////////////////////////////////////////////////

        [HttpGet("Listar/{id}")]
        public async Task<ActionResult<List<UsuarioPermisoDTO>>> Get(int id)
        {
            try
            {
                var query = from ur in _context.UsuarioPermisosPantallas
                            join r in _context.Pantallas on ur.IdPantalla equals r.IdPantalla
                            join p in _context.Permisos on ur.IdPermiso equals p.IdPermiso
                            join u in _context.Usuarios on ur.IdUsuario equals u.IdUsuario
                            join s in _context.Sistemas on r.IdPantalla equals s.IdSistema
                            where ur.IdUsuario == id
                            select new UsuarioPermisoDTO
                            {
                                IdPantalla = r.IdPantalla,
                                IdUsuario = u.IdUsuario,
                                IdPermiso = p.IdPermiso,
                                Sistema = s.Descripcion,
                                Pantalla = r.Descripcion,
                                Permiso = p.Descripcion
                            };

                var permisosRol = query.ToList();
                if (permisosRol == null) return NotFound();
                return Ok(permisosRol);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////

        [HttpPost("Guardar")]
        public async Task<ActionResult> Post([FromBody] UsuarioPermisoPantallaDTO usuarioPermisoPantalla)
        {
            using (var transaccion = _context.Database.BeginTransaction())
            {
                try
                {
                    UsuarioPermisoPantalla temp = new UsuarioPermisoPantalla
                    {
                        IdUsuario = usuarioPermisoPantalla.IdUsuario,
                        IdPermiso = usuarioPermisoPantalla.IdPermiso,
                        IdPantalla = usuarioPermisoPantalla.IdPantalla
                    };

                    await _context.UsuarioPermisosPantallas.AddAsync(temp);
                    await _context.SaveChangesAsync();

                    await transaccion.CommitAsync();

                    return Ok(temp);
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    return BadRequest(ex.Message);

                }
            }
        }
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete([FromBody] UsuarioPermisoPantallaDTO permiso)
        {
            using (var transaccion = _context.Database.BeginTransaction())
            {
                try
                {
                    var temp = await _context.UsuarioPermisosPantallas.FirstOrDefaultAsync(x => x.IdUsuario == permiso.IdUsuario && x.IdPantalla == permiso.IdPantalla && x.IdPermiso == permiso.IdPermiso);
                    if (temp == null) return NotFound();

                    _context.UsuarioPermisosPantallas.Remove(temp);
                    await _context.SaveChangesAsync();
                    // Confirma la transacción
                    await transaccion.CommitAsync();

                    return Ok("Permiso eliminado Correctamente.");
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    return BadRequest($"Error al eliminar: {ex.Message}");
                }
            }
        }
    }//class
}//namespace
