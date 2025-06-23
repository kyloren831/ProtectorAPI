using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtectorAPP.Models;
using ProtectorAPP.Models.Pantallas;
using ProtectorAPP.Models.Roles;
using ProtectorAPP.Models.Sistemas;

namespace ProtectorAPP.Controllers
{
    public class RolesController : Controller
    {
        private readonly HttpClient httpClient;
        public RolesController(IHttpClientFactory httpClient)
        {
            this.httpClient = httpClient.CreateClient("ApiClient");
        }

        // GET: RolesController
        public async Task< ActionResult > Index()
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var Roles = new List<RolViewModel>();
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await httpClient.GetAsync("Rol/Listar");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Roles = JsonConvert.DeserializeObject<List<RolViewModel>>(json);
                }
                else
                {
                    TempData["ErrorMessage"] = "Error en el servidor";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error en el servidor";
            }
            return View(Roles);
        }

        // POST: RolesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int idRol, string descripcion)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");
            try
            {
                if (idRol == null || idRol < 0 || string.IsNullOrEmpty(descripcion)) {
                    TempData["ErrorMessage"] = "Debe llenar todos los espacios con valores validos";
                    return RedirectToAction(nameof(Index));
                }

                var nRol = new RolViewModel { IdRol = idRol, Descripcion= descripcion };
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(nRol);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("Rol/Guardar", content);

                if (response.IsSuccessStatusCode) {
                    TempData["SuccessMessage"] = "Rol agregado correctamente";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Error en el servidor";
                    return RedirectToAction(nameof(Index));

                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: RolesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RolesController/Edit/5
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

        // POST: RolesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await httpClient.DeleteAsync($"Rol/{id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Rol eliminado correctamente";
                    return RedirectToAction(nameof(Index));

                }
                else
                {

                    TempData["ErrorMessage"] = "Error en el servidor";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
