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
    public class UsuariosController : ControllerBase
    {
        private readonly ProtectorDbContext context;

        public UsuariosController(ProtectorDbContext context)
        {
            this.context = context;
        }
        // GET: api/<UsuariosController>
        [HttpGet]
        public async Task<ActionResult<List<UsuarioDTO>>> Get()
        {
            try
            {
                var usuarios = await context.Usuarios.ToListAsync();
                List<UsuarioDTO> temp = new List<UsuarioDTO>();
                foreach (var usuario in usuarios)
                {
                    temp.Add(new UsuarioDTO
                    {
                        IdUsuario = usuario.IdUsuario,
                        Nombre = usuario.Nombre,
                        Correo = usuario.Correo,
                        Contrasenna = usuario.Contrasenna,
                        FechaCreacion = usuario.FechaCreacion,
                        FotoUrl = usuario.FotoUrl,
                        Estado = usuario.Estado
                    });
                }
                return Ok(temp);
            }
            catch(Exception ex) 
            {
                    return BadRequest(ex.Message);
            }
            
        }

        // GET api/<UsuariosController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDTO>> Get(int id)
        {
                try
                {
                    var usuario = await context.Usuarios.FindAsync(id);
                    UsuarioDTO temp = new UsuarioDTO
                    {
                        IdUsuario = usuario.IdUsuario,
                        Nombre = usuario.Nombre,
                        Correo = usuario.Correo,
                        Contrasenna = usuario.Contrasenna,
                        FechaCreacion = usuario.FechaCreacion,
                        FotoUrl = usuario.FotoUrl,
                        Estado = usuario.Estado
                    };
                    return Ok(temp);
                }catch (Exception ex)
                {
                      return BadRequest(ex.Message);
                }
           
        }

        // POST api/<UsuariosController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UsuarioDTO usuario)
        {
            //Se abre una trasaccion
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    Usuario temp = new Usuario
                    {
                        IdUsuario = usuario.IdUsuario,
                        Nombre = usuario.Nombre,
                        Correo = usuario.Correo,
                        Contrasenna = usuario.Contrasenna,
                        FechaCreacion = usuario.FechaCreacion,
                        FotoUrl = usuario.FotoUrl,
                        Estado = usuario.Estado
                    };

                    // se inserta el usuario y se guardan los cambios
                    await context.Usuarios.AddAsync(temp);
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

        // PUT api/<UsuariosController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Usuario>> Put(int id, [FromBody] UsuarioDTO usuario)
        {
            if (id != usuario.IdUsuario) //Si el id que del URL != id del body return bad request
                return BadRequest();


            //Se abre una trasaccion
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    var temp = await context.Usuarios.FindAsync(id);

                    // Si no se encuentra el usuario, retorna NotFound
                    if (temp == null)
                        return NotFound();

                    temp.Nombre = usuario.Nombre;
                    temp.Correo = usuario.Correo;
                    temp.Contrasenna = usuario.Contrasenna;
                    temp.FechaCreacion = usuario.FechaCreacion;
                    temp.FotoUrl = usuario.FotoUrl;
                    temp.Estado = usuario.Estado;
                   

                    // se inserta el usuario y se guardan los cambios
                    context.Usuarios.Update(temp);
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

        // DELETE api/<UsuariosController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
