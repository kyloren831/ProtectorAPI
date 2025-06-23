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

    public class RolController : ControllerBase
    {
        private readonly ProtectorDbContext context;

        public RolController(ProtectorDbContext context)
        {

            this.context = context;

        }

/////////////////////////////////////////////////////////////////////////////////

        [HttpGet("Listar")]
        [Authorize]
        public async Task<ActionResult<List<RolDTO>>> Get()
        {
            try
            {

                var roles = await context.Roles.ToListAsync();

                List<RolDTO> temp = new List<RolDTO>();

                foreach (var rol in roles)
                {
                    temp.Add(new RolDTO
                    {
                        IdRol = rol.IdRol,
                        Descripcion = rol.Descripcion
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
        [Authorize]
        public async Task<ActionResult> Post([FromBody] RolDTO rol)
        {
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    Rol temp = new Rol
                    {
                        IdRol = rol.IdRol,
                        Descripcion = rol.Descripcion
                    };

                    await context.Roles.AddAsync(temp);
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

/////////////////////////////////////////////////////////////////////////////////

        [HttpGet("Buscar")]
        [Authorize]
        public async Task<ActionResult<RolDTO>> Get(int id)
        {
            try
            {
                var rol = await context.Roles.FindAsync(id);
                RolDTO temp = new RolDTO  
                {
                    IdRol = rol.IdRol,
                    Descripcion = rol.Descripcion
                };
                return Ok(temp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

/////////////////////////////////////////////////////////////////////////////////

        [HttpPut("Actualizar")]
        [Authorize]
        public async Task<ActionResult<Rol>> Put(int id, [FromBody] RolDTO rol)
        {
            if (id != rol.IdRol)
                return BadRequest();

            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    var temp = await context.Roles.FindAsync(id);

                    if (temp == null)
                        return NotFound();

                    temp.Descripcion = rol.Descripcion;

                    context.Roles.Update(temp);
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

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Eliminar(int id)
        {
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    var temp = await context.Roles.FindAsync(id);
                    if (temp == null) return NotFound();

                    context.Roles.Remove(temp);
                    await context.SaveChangesAsync();
                    // Confirma la transacción
                    await transaccion.CommitAsync();

                    return Ok("Rol eliminado Correctamente.");
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    return BadRequest($"Error al cambiar el estado: {ex.Message}");
                }
            }
        }

    }//class
}//namespace
