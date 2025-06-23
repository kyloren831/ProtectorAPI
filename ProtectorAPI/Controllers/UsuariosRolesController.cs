using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtectorAPI.Models;
using ProtectorAPI.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProtectorAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace ProtectorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosRolesController : ControllerBase
    {
        private readonly ProtectorDbContext _context;

        public UsuariosRolesController(ProtectorDbContext context)
        {
            _context = context;
        }

        // POST: api/UsuariosRoles
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UsuarioRol>> PostUsuarioRol(UsuarioRolDTO usuarioRol)
        {
            using (var transaccion = _context.Database.BeginTransaction())
            {
                try
                {
                    var temp = new UsuarioRol
                    {
                        IdRol = usuarioRol.IdRol,
                        IdUsuario = usuarioRol.IdUsuario
                    };

                    await _context.UsuarioRoles.AddAsync(temp);
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

        // GET: api/UsuariosRoles
        [HttpGet("Lista/{id}")]
        [Authorize]
        public ActionResult<IEnumerable<GetUsuarioRolDTO>> GetUsuariosRoles(int id)
        {
            try
            {
                var query = from ur in _context.UsuarioRoles
                            join r in _context.Roles on ur.IdRol equals r.IdRol
                            join u in _context.Usuarios on ur.IdUsuario equals u.IdUsuario
                            where ur.IdUsuario == id
                            select new GetUsuarioRolDTO
                            {
                                IdRol = r.IdRol,
                                IdUsuario = u.IdUsuario,
                                Rol = r.Descripcion,
                                Usuario = u.Nombre
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

        // DELETE: api/UsuariosRoles/5
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete([FromBody] UsuarioRolDTO rol)
        {
            using (var transaccion = _context.Database.BeginTransaction())
            {
                try
                {
                    var temp = await _context.UsuarioRoles.FirstOrDefaultAsync(x => x.IdRol == rol.IdRol && x.IdUsuario == rol.IdUsuario);
                    if (temp == null) return NotFound();

                    _context.UsuarioRoles.Remove(temp);
                    await _context.SaveChangesAsync();
                    // Confirma la transacción
                    await transaccion.CommitAsync();

                    return Ok("Rol eliminado Correctamente.");
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    return BadRequest($"Error al eliminar: {ex.Message}");
                }
            }
        }
    }
}