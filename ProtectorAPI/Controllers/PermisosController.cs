using Microsoft.AspNetCore.Mvc;
using ProtectorAPI.Data;
using ProtectorAPI.DTOs;
using ProtectorAPI.Models;
using ProtectorAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProtectorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermisosController : ControllerBase
    {
        private readonly ProtectorDbContext context;
        private readonly IUserService userService;

        public PermisosController(ProtectorDbContext context, IUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }

        // GET: api/<PermisosController>
        [HttpGet]
        public IEnumerable<Permiso> Get()
        {
            IEnumerable<Permiso> permisos = context.Permisos.ToList();
            return permisos; 
        }

        [HttpGet("TotalPermisos")]
        public async Task<IEnumerable<PantallaConPermisosDTO>> GetTotalPantallas()
        {
            IEnumerable<PantallaConPermisosDTO> permisos = await userService.ObtenerPantallasConTodosLosPermisos();
            return permisos;
        }


    }
}
