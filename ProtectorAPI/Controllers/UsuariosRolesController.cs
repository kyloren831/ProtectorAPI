using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtectorAPI.Models;
using ProtectorAPI.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<ActionResult<UsuarioRol>> PostUsuarioRol(UsuarioRol usuarioRol)
        {
            // Agrega un nuevo registro de UsuarioRol a la base de datos
            _context.UsuarioRoles.Add(usuarioRol);
            await _context.SaveChangesAsync();

            // Devuelve la ubicación del nuevo recurso creado
            return CreatedAtAction("GetUsuarioRol", new { id = usuarioRol.IdRol }, usuarioRol);
        }

        // GET: api/UsuariosRoles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioRol>>> GetUsuariosRoles()
        {
            // Obtiene todos los registros de UsuarioRol junto con los detalles de Usuario y Rol
            return await _context.UsuarioRoles
                .Include(u => u.Usuario)
                .Include(r => r.Rol)
                .ToListAsync();
        }

        // GET: api/UsuariosRoles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioRol>> GetUsuarioRol(int id)
        {
            // Obtiene un solo registro de UsuarioRol por su IdRol
            var usuarioRol = await _context.UsuarioRoles
                .Include(u => u.Usuario)
                .Include(r => r.Rol)
                .FirstOrDefaultAsync(m => m.IdRol == id);

            if (usuarioRol == null)
            {
                // Si no se encuentra, retorna un 404
                return NotFound();
            }

            return usuarioRol;
        }

        // PUT: api/UsuariosRoles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuarioRol(int id, UsuarioRol usuarioRol)
        {
            // Verifica que el id proporcionado coincida con el id del recurso a actualizar
            if (id != usuarioRol.IdRol)
            {
                return BadRequest();
            }

            // Marca el objeto como modificado
            _context.Entry(usuarioRol).State = EntityState.Modified;

            try
            {
                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Si ocurre un error de concurrencia o si no existe el registro
                if (!UsuarioRolExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Devuelve un estado 204 (sin contenido) indicando que la actualización fue exitosa
            return NoContent();
        }

        // DELETE: api/UsuariosRoles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuarioRol(int id)
        {
            // Busca el registro a eliminar
            var usuarioRol = await _context.UsuarioRoles.FindAsync(id);
            if (usuarioRol == null)
            {
                // Si no se encuentra, retorna un 404
                return NotFound();
            }

            // Elimina el registro
            _context.UsuarioRoles.Remove(usuarioRol);
            await _context.SaveChangesAsync();

            // Devuelve un estado 204 (sin contenido) indicando que la eliminación fue exitosa
            return NoContent();
        }

        // Método auxiliar para verificar si un UsuarioRol existe
        private bool UsuarioRolExists(int id)
        {
            return _context.UsuarioRoles.Any(e => e.IdRol == id);
        }
    }
}