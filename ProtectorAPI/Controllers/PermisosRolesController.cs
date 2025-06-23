using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtectorAPI.Data;
using ProtectorAPI.DTOs;
using ProtectorAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProtectorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermisosRolesController : ControllerBase
    {
        private readonly ProtectorDbContext context;

        public PermisosRolesController(ProtectorDbContext context)
        {

            this.context = context;

        }
       
        [HttpGet("ConPermisos")]
        [Authorize]
        public async Task<ActionResult<List<RolConPermisosDTO>>> GetConPermisos([FromQuery] int id)
        {
            try
            {
                var query = from pr in context.RolPermisosPantallas
                                  join r in context.Roles on pr.IdRol equals r.IdRol
                                  join p in context.Pantallas on pr.IdPantalla equals p.IdPantalla
                                  join pe in context.Permisos on pr.IdPermiso equals pe.IdPermiso
                                  where pr.IdRol == id
                                  select new RolConPermisosDTO
                                  {
                                      IdRol = r.IdRol,
                                      IdPantalla = p.IdPantalla,
                                      IdPermiso = pe.IdPermiso,
                                      Rol = r.Descripcion,
                                      Pantalla = p.Descripcion,
                                      Permiso = pe.Descripcion
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

       

        // POST api/<PermisosRolesController>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Post([FromBody]  PermisoRolDTO permiso)
        {
            using (var transaccion = context.Database.BeginTransaction()) //manejo de concurrencia
            {
                try
                {
                    var temp = new RolPermisoPantalla
                    {
                        IdPantalla = permiso.IdPantalla,
                        IdRol = permiso.IdRol,
                        IdPermiso = permiso.IdPermiso
                    };

                    await context.RolPermisosPantallas.AddAsync(temp);
                    await context.SaveChangesAsync();

                    await transaccion.CommitAsync();

                    return Ok(permiso);
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    return BadRequest(ex.Message);

                }
            }
        }

        // DELETE api/<PermisosRolesController>/5
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult> Delete([FromBody] PermisoRolDTO permiso)
        {
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    var temp = await context.RolPermisosPantallas.FirstOrDefaultAsync(x=>x.IdRol==permiso.IdRol && x.IdPantalla==permiso.IdPantalla && x.IdPermiso==permiso.IdPermiso);
                    if (temp == null) return NotFound();

                    context.RolPermisosPantallas.Remove(temp);
                    await context.SaveChangesAsync();
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
    }
}
