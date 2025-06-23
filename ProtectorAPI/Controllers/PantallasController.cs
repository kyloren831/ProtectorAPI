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
    public class PantallasController : ControllerBase
    {
        private readonly ProtectorDbContext context;

        public PantallasController(ProtectorDbContext context)
        {
            this.context = context;
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Pantalla>>> Get()
        {
            try
            {
                var temp = await context.Pantallas.ToListAsync();
                if (temp == null) return NotFound();
                return Ok(temp);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        // GET api/<PantallasController>/5
        [HttpGet("{id}")]
        [Authorize]
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

        [Authorize]
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
        // Patch api/<UsuariosController>/5
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<ActionResult> CambiarEstado(int id)
        {
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    var temp = await context.Pantallas.FindAsync(id);
                    if (temp == null) return NotFound();

                    if (temp.Estado.ToString().Equals("A"))
                    {
                        temp.Estado = char.Parse("B");
                    }
                    else
                    {
                        temp.Estado = char.Parse("A");
                    }

                    context.Pantallas.Update(temp);
                    await context.SaveChangesAsync();

                    // Confirma la transacción
                    await transaccion.CommitAsync();

                    return Ok(temp);
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    return BadRequest($"Error al cambiar el estado: {ex.Message}");
                }
            }
        }

        // PUT api/<PantallasController>/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> Put(int id, [FromBody] PantallaDTO dto)
        {
            if(id != dto.IdPantalla)
            {
                return BadRequest("Datos invalidos...");
            }
            //Se abre una trasaccion
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    var temp = await context.Pantallas.FindAsync(id);

                    // Si no se encuentra el usuario, retorna NotFound
                    if (temp == null)
                        return NotFound();

                    temp.Descripcion = dto.Descripcion;
                    temp.Estado = dto.Estado;
                    temp.FotoUrl = dto.FotoUrl;


                    // se inserta el usuario y se guardan los cambios
                    context.Pantallas.Update(temp);
                    await context.SaveChangesAsync();

                    // Confirma la transacción
                    await transaccion.CommitAsync();

                    return Ok(temp);

                }
                catch (Exception ex)
                {
                    // deshace la transacción si ocurre un error 
                    await transaccion.RollbackAsync();
                    return BadRequest(ex.Message);
                }
            }
        }

        // DELETE api/<PantallasController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
