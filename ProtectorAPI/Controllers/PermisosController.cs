using Microsoft.AspNetCore.Mvc;
using ProtectorAPI.Data;
using ProtectorAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProtectorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermisosController : ControllerBase
    {
        private readonly ProtectorDbContext context;

        public PermisosController(ProtectorDbContext context)
        {
            this.context = context;
        }

        // GET: api/<PermisosController>
        [HttpGet]
        public IEnumerable<Permiso> Get()
        {
            IEnumerable<Permiso> permisos = context.Permisos.ToList();
            return permisos; 
        }



    }
}
