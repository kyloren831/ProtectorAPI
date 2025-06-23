using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtectorAPI.Data;
using ProtectorAPI.DTOs;
using ProtectorAPI.Models;

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
        // Patch api/<UsuariosController>/5
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<ActionResult> CambiarEstado(int id)
        {
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    var temp = await context.Sistemas.FindAsync(id);
                    if (temp == null) return NotFound();

                    if (temp.Estado.ToString().Equals("A"))
                    {
                        temp.Estado = "B";
                    }
                    else
                    {
                        temp.Estado = "A";
                    }

                    context.Sistemas.Update(temp);
                    await context.SaveChangesAsync();

                    // Confirma la transacción
                    await transaccion.CommitAsync();

                    return Ok("Estado cambiado exitosamente.");
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    return BadRequest($"Error al cambiar el estado: {ex.Message}");
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpGet("Listar/ConPantallas")]
        [Authorize]
        public async Task<ActionResult<Sistema>> GetConPantallas([FromQuery] int id)
        {
            try
            {
                var sistema = await context.Sistemas.Include(x => x.Pantallas).FirstOrDefaultAsync(x => x.IdSistema == id);
                if (sistema == null) return NotFound();

                return Ok(sistema);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("Listar/TodasConPantallas")]
        [Authorize]
        public async Task<ActionResult<Sistema>> GetConPantallas()
        {
            try
            {
                var sistema = await context.Sistemas.Include(x => x.Pantallas).ToListAsync();
                if (sistema == null) return NotFound();
                return Ok(sistema);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpGet("Listar")]
        [Authorize]
        public async Task<ActionResult<List<SistemaDTO>>> Get()
        {
            try
            {
                var sistemas = await context.Sistemas.ToListAsync();

                List<SistemaDTO> temp = new List<SistemaDTO>();

                foreach (var sistema in sistemas)
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
        [Authorize]
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
        [Authorize]
        public async Task<ActionResult<SistemaDTO>> Get(int id)
        {
            try
            {
                var sistema = await context.Sistemas.FirstOrDefaultAsync(x => x.IdSistema == id);
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
        [HttpPost("Buscar/ParaInicio")]
        [Authorize]
        public async Task<ActionResult<List<SistemaDTO>>> Post(List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest("No se proporcionaron IDs.");
            }

            try
            {
                var sistemas = await context.Sistemas
                    .Where(x => ids.Contains(x.IdSistema)) // Obtener todos los sistemas en una sola consulta
                    .Select(x => new SistemaDTO
                    {
                        IdSistema = x.IdSistema,
                        Descripcion = x.Descripcion,
                        Url = x.Url,
                        Estado = x.Estado
                    })
                    .ToListAsync(); // Ejecuta la consulta y obtiene la lista de resultados

                if (sistemas == null || !sistemas.Any())
                {
                    return NotFound("No se encontraron sistemas con los IDs proporcionados.");
                }

                return Ok(sistemas);
            }
            catch (Exception ex)
            {
                // Aquí puedes capturar excepciones específicas si lo deseas.
                // Log de errores (por ejemplo, loggers o herramientas de monitoreo de excepciones)
                return StatusCode(500, $"Ocurrió un error interno: {ex.Message}");
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpPut("Actualizar")]
        [Authorize]
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

    }//class
}//namespace
