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
    public class PantallasController : ControllerBase
    {
        private readonly ProtectorDbContext context;

        public PantallasController(ProtectorDbContext context)
        {
            this.context = context;
        }

       

        // GET api/<PantallasController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pantalla>> Get(int id)
        {
            try
            {
                var temp = await context.Pantallas.FirstOrDefaultAsync(x=> x.IdPantalla == id);
                if (temp == null) return NotFound();
                return Ok(temp);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<PantallasController>
        [HttpPost]
        public async Task<ActionResult<Pantalla>> Post([FromBody] PantallaDTO dto)
        {
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    if (!ModelState.IsValid) return BadRequest();

                    Pantalla pantalla = new Pantalla
                    {
                        IdPantalla = dto.IdPantalla,
                        IdSistema = dto.IdSistema,
                        Descripcion = dto.Descripcion,
                        FotoUrl = dto.FotoUrl,
                        Estado = dto.Estado
                    };
                    await context.Pantallas.AddAsync(pantalla);
                    await context.SaveChangesAsync();

                    // Confirma la transacción
                    await transaccion.CommitAsync();

                    return Ok(pantalla);

                }
                catch (Exception ex)
                {
                    // deshace la transacción si ocurre un error 
                    await transaccion.RollbackAsync();
                    return BadRequest(ex.Message);
                }
            }
        }

        // PUT api/<PantallasController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PantallasController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
