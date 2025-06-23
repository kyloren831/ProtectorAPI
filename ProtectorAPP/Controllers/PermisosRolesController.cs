using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtectorAPP.Models.Pantallas;
using ProtectorAPP.Models.Roles;
using ProtectorAPP.Models;
using System.Text;
using System.Runtime.CompilerServices;

namespace ProtectorAPP.Controllers
{
    public class PermisosRolesController : Controller
    {
        private readonly HttpClient httpClient;
        public PermisosRolesController(IHttpClientFactory httpClient)
        {
            this.httpClient = httpClient.CreateClient("ApiClient");
        }
        // GET: PermisosRolesController
        public async Task<ActionResult> Index(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await httpClient.GetAsync($"PermisosRoles/ConPermisos?id={id}");
                var responsePantallas = await httpClient.GetAsync($"Pantallas");
                var responsePermisos = await httpClient.GetAsync($"Permisos");

                if (response.IsSuccessStatusCode && responsePantallas.IsSuccessStatusCode && responsePermisos.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var jsonPantallas = await responsePantallas.Content.ReadAsStringAsync();
                    var jsonPermisos = await responsePermisos.Content.ReadAsStringAsync();
                    var rolPermisos = JsonConvert.DeserializeObject<List<RolConPermisosViewModel>>(json);
                    var pantallas = JsonConvert.DeserializeObject<List<PantallaViewModel>>(jsonPantallas);
                    var permisos = JsonConvert.DeserializeObject<List<PermisoDTO>>(jsonPermisos);

                    var model = new IndexViewModel
                    {
                        IdRol=id,
                        RolConPermisos = rolPermisos,
                        Pantallas=pantallas,
                        Permisos=permisos
                    };

                    return View(model);
                }
                else
                {
                    TempData["ErrorMessage"] = "Error en el servidor";

                    return RedirectToAction("Index", "Roles");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error en el servidor";

                return RedirectToAction("Index", "Roles");
            }
        }

        // GET: PermisosRolesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }


        // POST: PermisosRolesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int idRol, int idPantalla, int idPermiso)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var permiso = new PermisosRol
                {
                    IdPantalla = idPantalla,
                    IdPermiso = idPermiso,
                    IdRol = idRol
                };
                var json = JsonConvert.SerializeObject(permiso);
                var content = new StringContent(json, Encoding.UTF8, "application/json"); // Crear el contenido de la solicitud
                var response = await httpClient.PostAsync($"PermisosRoles",content);

                if (response.IsSuccessStatusCode) 
                {
                    TempData["SuccessMessage"] = "Permisos otorgados correctamente";
                    return RedirectToAction(nameof(Index), new { id = idRol });
                }
                else
                {
                    TempData["ErrorMessage"] = "Error en el servidor";
                    return RedirectToAction(nameof(Index), new { id = idRol });
                }
            }
            catch
            {

                TempData["ErrorMessage"] = "Error en el servidor";
                return RedirectToAction(nameof(Index), new { id = idRol });
            }
        }

        // GET: PermisosRolesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PermisosRolesController/Edit/5
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
        // POST: PermisosRolesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult> Delete(int idRol, int idPantalla, int idPermiso)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var permiso = new PermisosRol
                {
                    IdPantalla = idPantalla,
                    IdPermiso = idPermiso,
                    IdRol = idRol
                };

                var json = JsonConvert.SerializeObject(permiso);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Crear una solicitud HTTP DELETE con el contenido
                var request = new HttpRequestMessage(HttpMethod.Delete, "https://localhost:7293/api/PermisosRoles")
                {
                    Content = content
                };

                // Hacer la solicitud
                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Permiso eliminado correctamente";
                    return RedirectToAction(nameof(Index), new { id = idRol });

                }
                else
                {

                    TempData["ErrorMessage"] = "Error en el servidor";
                    return RedirectToAction(nameof(Index), new { id = idRol });
                }
            }
            catch (Exception ex)
            {

                TempData["ErrorMessage"] = "Error en el servidor";
                return RedirectToAction(nameof(Index), new { id = idRol });
            }
        }
    }
}
