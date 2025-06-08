using Microsoft.AspNetCore.Mvc;
using ProtectorAPI.DTOs;
using ProtectorAPI.Models;
using ProtectorAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace ProtectorAPI.Controllers
{

    [Route("api/[controller]")]

    [ApiController]

    public class UsuarioPermisoPantallaController : ControllerBase
    {

        private readonly ProtectorDbContext context;

        public UsuarioPermisoPantallaController(ProtectorDbContext context)
        {

            this.context = context;

        }

/////////////////////////////////////////////////////////////////////////////////

        [HttpGet("Listar")]

        public async Task<ActionResult<List<UsuarioPermisoPantallaDTO>>> Get()
        {

            try
            {

                var UsuarioPermisosPantallas = await context.UsuarioPermisosPantallas.ToListAsync();

                List<UsuarioPermisoPantallaDTO> temp = new List<UsuarioPermisoPantallaDTO>();

                foreach (var UsuarioPermisoPantalla in UsuarioPermisosPantallas)
                {

                    temp.Add(new UsuarioPermisoPantallaDTO
                    {
                        IdUsuario = UsuarioPermisoPantalla.IdUsuario,
                        IdPermiso = UsuarioPermisoPantalla.IdPermiso,
                        IdPantalla = UsuarioPermisoPantalla.IdPantalla
                    });
                }

                return Ok(temp);
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

            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {

                    UsuarioPermisoPantalla temp = new UsuarioPermisoPantalla
                    {
                        IdUsuario = usuarioPermisoPantalla.IdUsuario,
                        IdPermiso = usuarioPermisoPantalla.IdPermiso,
                        IdPantalla = usuarioPermisoPantalla.IdPantalla
                    };

                    await context.UsuarioPermisosPantallas.AddAsync(temp);
                    await context.SaveChangesAsync();

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

//////////////////////////////////////////////////////////////////////////////////

        [HttpDelete("Eliminar")]
        public async Task<ActionResult> Delete([FromQuery] int idUsuario, [FromQuery] int idPermiso, [FromQuery] int idPantalla)
        {
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    var temp = await context.UsuarioPermisosPantallas.FirstOrDefaultAsync(temp =>
                        temp.IdUsuario == idUsuario &&
                        temp.IdPermiso == idPermiso &&
                        temp.IdPantalla == idPantalla);

                    if (temp == null)
                        return NotFound("UsuarioPermisoPantalla no encontrado.");

                    context.UsuarioPermisosPantallas.Remove(temp);
                    await context.SaveChangesAsync();

                    await transaccion.CommitAsync();

                    return Ok("UsuarioPermisoPantalla eliminado correctamente.");
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    return BadRequest($"Error al eliminar el UsuarioPermisoPantalla: {ex.Message}");
                }
            }
        }

    }//class
}//namespace
