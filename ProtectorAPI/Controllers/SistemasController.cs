using Microsoft.AspNetCore.Mvc;

using ProtectorAPI.Models;
using ProtectorAPI.Data;
using ProtectorAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ProtectorAPI.Controllers
{

    [Route("api/[controller]")]

    [ApiController]

    public class SistemasController : ControllerBase
    {

        private readonly ProtectorDbContext context;

        public SistemasController(ProtectorDbContext context)
        {

            this.context = context;

        }

//////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpGet("Listar")]
        public async Task<ActionResult<List<SistemaDTO>>> Get()
        {
            try
            {
                var sistemas = await context.Sistemas.ToListAsync();

                List<SistemaDTO> temp = new List<SistemaDTO>();

                foreach(var sistema in sistemas)
                {
                    temp.Add(new SistemaDTO
                    {
                        IdSistema = sistema.IdSistema,
                        Descripcion = sistema.Descripcion,
                        Url = sistema.Url,
                        Estado = sistema.Estado
                    });
                }

                return Ok(temp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

//////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpPost("Guardar")]
        public async Task<ActionResult> Post([FromBody] SistemaDTO sistema)
        {
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    Sistema temp = new Sistema
                    {
                        IdSistema = sistema.IdSistema, 
                        Descripcion = sistema.Descripcion,
                        Url = sistema.Url,
                        Estado = sistema.Estado
                    };

                    await context.Sistemas.AddAsync(temp);
                    await context.SaveChangesAsync();

                    await transaccion.CommitAsync();

                    return Ok(temp);
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);

                }
            }
        }

//////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        [HttpGet("Buscar")]
        public async Task<ActionResult<SistemaDTO>> Get(int id)
        {
            try
            {
                var sistema = await context.Sistemas.FindAsync(id);
                SistemaDTO temp = new SistemaDTO
                {
                    IdSistema = sistema.IdSistema,
                    Descripcion = sistema.Descripcion,
                    Url = sistema.Url,
                    Estado = sistema.Estado
                };
                return Ok(temp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

//////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpPut("Actualizar")]
        public async Task<ActionResult<Sistema>> Put(int id, [FromBody] SistemaDTO sistema)
        {
            if (id != sistema.IdSistema)
                return BadRequest();

            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    var temp = await context.Sistemas.FindAsync(id);

                    if (temp == null)
                        return NotFound();

                    temp.Descripcion = sistema.Descripcion;
                    temp.Url = sistema.Url;
                    temp.Estado = sistema.Estado;

                    context.Sistemas.Update(temp);
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

//////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpDelete("Eliminar")]
        public async Task<ActionResult> Delete([FromQuery] int id)
        {
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    var temp = await context.Sistemas.FindAsync(id);

                    if (temp == null)
                        return NotFound("Sistema no encontrado.");

                    context.Sistemas.Remove(temp);
                    await context.SaveChangesAsync();

                    await transaccion.CommitAsync();

                    return Ok("Sistema eliminado correctamente.");
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    return BadRequest($"Error al eliminar el Sistema: {ex.Message}");
                }
            }
        }

    }//class
}//namespace
