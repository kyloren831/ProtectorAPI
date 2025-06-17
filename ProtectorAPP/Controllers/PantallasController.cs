using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtectorAPP.Models;

namespace ProtectorAPP.Controllers
{
    public class PantallasController : Controller
    {
        private readonly HttpClient httpClient;
        public PantallasController(IHttpClientFactory httpClient)
        {
            this.httpClient = httpClient.CreateClient("ApiClient");
        }
        // GET: PantallasController
        public async Task<ActionResult> Index(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await httpClient.GetAsync($"Sistemas/Listar/ConPantallas?id={id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var sistema = JsonConvert.DeserializeObject<SistemaConPantallasViewModel>(json);
                    return View(sistema);
                }
                else
                {
                    TempData["ErrorMessage"] = "Error en el servidor";

                    return RedirectToAction("Index", "Sistemas");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error en el servidor";

                return RedirectToAction("Index", "Sistemas");
            }
            
        }

        // GET: PantallasController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PantallasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PantallasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PantallasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PantallasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PantallasController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PantallasController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
