using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtectorAPI.Data;
using ProtectorAPI.DTOs;
using ProtectorAPI.Models;
using ProtectorAPI.Services;
using LoginRequest = ProtectorAPI.DTOs.LoginRequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProtectorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ProtectorDbContext context;
        private readonly IEmailService servicioEmail;
        private readonly IAuthorizationService authorizationServices;

        public UsuariosController(ProtectorDbContext context, IEmailService servicioEmail, IAuthorizationService authorizationServices)
        {
            this.context = context;
            this.servicioEmail = servicioEmail;
            this.authorizationServices = authorizationServices;
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

                    var contrasenna = PasswordGenerator.Generar(10);
                    temp.Contrasenna = contrasenna;
                   

                    var passwordHasher = new PasswordHasher<Usuario>();
                    //la contrasena se hashea  antes de guardarla
                    temp.Contrasenna = passwordHasher.HashPassword(temp, temp.Contrasenna);

                    // se inserta el usuario y se guardan los cambios
                    await context.Usuarios.AddAsync(temp);
                    await context.SaveChangesAsync();

                    // Confirma la transacción
                    await transaccion.CommitAsync();

                    //Si la transaccion sale bien, se envia el correo con la contrasenia temporal
                    string cuerpo = $@"
                                    <h3>Confirmacion de correo electronico</h3>
                                    <p>Solo funcionara la primera vez que ingrese al sistema.</p>
                                    <p><strong>Contraseña temporal:</strong> {contrasenna}</p>";

                    await servicioEmail.EnviarEmail(temp.Correo, "Confirmacion de correo electronico", cuerpo);

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
        public async Task<ActionResult<Usuario>> Put(int id, [FromBody] PutUsuarioDTO usuario)
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

        // Patch api/<UsuariosController>/5
        [HttpPatch("{id}")]
        public async Task<ActionResult> CambiarEstado(int id)
        {
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    var temp = await context.Usuarios.FindAsync(id);
                    if ( temp == null) return NotFound();

                    if (temp.Estado.Equals("A"))
                    {
                        temp.Estado = char.Parse("B");
                    }
                    else
                    {
                        temp.Estado = char.Parse("A");
                    }

                    context.Usuarios.Update(temp);
                    await context.SaveChangesAsync();

                    // Confirma la transacción
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

        // PATCH api/Usuarios/{id}/ChangePass
        [HttpPatch("{id}/ChangePass")]
        public async Task<ActionResult> CambiarContrasenna(int id, [FromBody] string nuevaContrasenna)
        {
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    var usuario = await context.Usuarios.FindAsync(id);
                    if (usuario == null)
                        return NotFound();

                    // Se hashea la nueva contraseña antes de guardarla
                    var passwordHasher = new PasswordHasher<Usuario>();
                    usuario.Contrasenna = passwordHasher.HashPassword(usuario, nuevaContrasenna);

                    context.Usuarios.Update(usuario);
                    await context.SaveChangesAsync();

                    await transaccion.CommitAsync();

                    return Ok(new { mensaje = "Contraseña actualizada exitosamente." });
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    return BadRequest(ex.Message);
                }
            }

        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Autenticar([FromBody] LoginRequest loginRequest)
        {
            var autorizado = await authorizationServices.GetToken(loginRequest.Email, loginRequest.Password);
            if (autorizado == null)
            {
                return Unauthorized();
            }
            else
            {
                return Ok(autorizado);
            }
        }
    }
}
