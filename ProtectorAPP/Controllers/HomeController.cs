using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtectorAPP.Models;
using ProtectorAPP.Models.Pantallas;
using ProtectorAPP.Models.Sistemas;
using System.Text;

namespace ProtectorAPP.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient httpClient;
        public HomeController(IHttpClientFactory httpClient)
        {
            this.httpClient = httpClient.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index()
        {
            
            if (!User.Identity.IsAuthenticated)
            {
                // Si no hay token, redirige al login
                return RedirectToAction("Login", "Auth");
            }

            var pantallasConPermisosJson = User.Claims.FirstOrDefault(c => c.Type == "PantallasConPermisos")?.Value;
            var pantallasConPermisos = JsonConvert.DeserializeObject<List<PantallaConPermisosDTO>>(pantallasConPermisosJson);

            try
            {
                List<int> ids = new List<int>();
                foreach (var item in pantallasConPermisos)
                {
                    ids.Add(item.IdSistema);
                }
                var token = HttpContext.Session.GetString("JwtToken");
                if (string.IsNullOrEmpty(token))
                {
                    ViewData["Error"] = "Token de autenticación no encontrado.";
                    return RedirectToAction("Login", "Auth");
                }
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(ids); // Serializar el objeto usuario en JSON
                var content = new StringContent(json, Encoding.UTF8, "application/json"); // Crear el contenido de la solicitud
                var response = await httpClient.PostAsync("Sistemas/Buscar/ParaInicio", content);
                if (response.IsSuccessStatusCode)
                {
                    var jsonRespuesta = await response.Content.ReadAsStringAsync();
                    var sistemas = JsonConvert.DeserializeObject<List<SistemasViewModel>>(jsonRespuesta);
                    return View(sistemas);
                }
                else
                {
                    ViewData["Error"] = "Error en el servidor";
                    return View();
                }

            }
            catch (Exception)
            {
                return View();
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Redirigir(string urlSistema)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (!string.IsNullOrEmpty(token))
            {
                var url = $"{urlSistema}/Home/Index?token={token}";
                return Redirect(url);
            }

            return BadRequest("Token no encontrado en sesión.");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
