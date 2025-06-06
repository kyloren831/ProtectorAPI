using Microsoft.AspNetCore.Mvc;

namespace ProtectorAPI.Controllers
{
    public class UsuariosRolesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
