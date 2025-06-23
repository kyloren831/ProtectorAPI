using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtectorAPP.Models;
using ProtectorAPP.Models.Pantallas;
using ProtectorAPP.Models.Roles;
using ProtectorAPP.Models.Usuarios;
using ProtectorAPP.Models.UsuariosPermisos;

namespace ProtectorAPP.Controllers
{
    public class UsuariosPermisosController : Controller
    {
        private readonly HttpClient httpClient;
        public UsuariosPermisosController(IHttpClientFactory httpClient)
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
                var responseDirectos = await httpClient.GetAsync($"UsuarioPermisoPantalla/Listar/{id}");

                var responseRoles = await httpClient.GetAsync($"UsuariosRoles/Lista/{id}");

                var responseUsuario = await httpClient.GetAsync($"Usuarios/{id}");

                var responsePantallas = await httpClient.GetAsync($"Pantallas");
                var responsePermisos = await httpClient.GetAsync($"Permisos");

                var responseRolesTotal = await httpClient.GetAsync("Rol/Listar");

                if (responseDirectos.IsSuccessStatusCode && responseRoles.IsSuccessStatusCode && responsePantallas.IsSuccessStatusCode && responsePermisos.IsSuccessStatusCode)
                {
                    var jsonDirectos = await responseDirectos.Content.ReadAsStringAsync();
                    var jsonRoles = await responseRoles.Content.ReadAsStringAsync();

                    var jsonUsuario = await responseUsuario.Content.ReadAsStringAsync();

                    var jsonPantallas = await responsePantallas.Content.ReadAsStringAsync();
                    var jsonPermisos = await responsePermisos.Content.ReadAsStringAsync();
                    var jsonRolesTotales = await responseRolesTotal.Content.ReadAsStringAsync();
                    var pantallas = JsonConvert.DeserializeObject<List<PantallaViewModel>>(jsonPantallas);
                    var permisos = JsonConvert.DeserializeObject<List<PermisoDTO>>(jsonPermisos);
                    var directos = JsonConvert.DeserializeObject<List<UsuarioPermisoDTO>>(jsonDirectos);
                    var roles = JsonConvert.DeserializeObject<List<GetUsuarioRolDTO>>(jsonRoles);
                    var rolesTotales = JsonConvert.DeserializeObject<List<RolViewModel>>(jsonRolesTotales);
                    var usuario = JsonConvert.DeserializeObject<UsuarioViewModel>(jsonUsuario);

                    var model = new Models.UsuariosPermisos.IndexViewModel
                    {
                        UsuariosPermisos = directos,
                        UsuarioRoles = roles,
                        Usuario = usuario,
                        Pantallas= pantallas,
                        Permisos = permisos,
                        Roles= rolesTotales
                    };

                    return View(model);
                }
                else
                {
                    TempData["ErrorMessage"] = "Error en el servidor";

                    return RedirectToAction("Index", "Usuarios");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error en el servidor";

                return RedirectToAction("Index", "Usuarios");
            }
        }

        // GET: UsuariosPermisosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        

        // POST: UsuariosPermisosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult > Create(int idUsuario, int idPantalla, int idPermiso)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var permiso = new PostUsuarioPermisoViewModel
                {
                    IdPantalla = idPantalla,
                    IdPermiso = idPermiso,
                    IdUsuario = idUsuario
                };

                var json = JsonConvert.SerializeObject(permiso);
                var content = new StringContent(json, Encoding.UTF8, "application/json"); // Crear el contenido de la solicitud
                var response = await httpClient.PostAsync($"UsuarioPermisoPantalla/Guardar", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Permisos otorgados correctamente";
                    return RedirectToAction(nameof(Index), new { id = idUsuario });
                }
                else
                {
                    TempData["ErrorMessage"] = "El usuario ya cuenta con estos permisos";
                    return RedirectToAction(nameof(Index), new { id = idUsuario });
                }
            }
            catch
            {

                TempData["ErrorMessage"] = "Error en el servidor";
                return RedirectToAction(nameof(Index), new { id = idUsuario });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateRol(int idUsuario, int idRol)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var permiso = new PostUsuarioRolDTO
                {
                    IdRol = idRol,
                    IdUsuario = idUsuario
                };

                var json = JsonConvert.SerializeObject(permiso);
                var content = new StringContent(json, Encoding.UTF8, "application/json"); // Crear el contenido de la solicitud
                var response = await httpClient.PostAsync($"UsuariosRoles", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Rol otorgado correctamente";
                    return RedirectToAction(nameof(Index), new { id = idUsuario });
                }
                else
                {
                    TempData["ErrorMessage"] = "El usuario ya tiene este rol asignado";
                    return RedirectToAction(nameof(Index), new { id = idUsuario });
                }
            }
            catch
            {

                TempData["ErrorMessage"] = "Error en el servidor";
                return RedirectToAction(nameof(Index), new { id = idUsuario });
            }
        }

        // GET: UsuariosPermisosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsuariosPermisosController/Edit/5
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int idUsuario, int idPantalla, int idPermiso)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var permiso = new 
                {
                    IdUsuario = idUsuario,
                    IdPermiso = idPermiso,
                    IdPantalla = idPantalla
                };

                var json = JsonConvert.SerializeObject(permiso);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Crear una solicitud HTTP DELETE con el contenido
                var request = new HttpRequestMessage(HttpMethod.Delete, "https://localhost:7293/api/UsuarioPermisoPantalla")
                {
                    Content = content
                };

                // Hacer la solicitud
                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Permiso eliminado correctamente";
                    return RedirectToAction(nameof(Index), new { id = idUsuario });

                }
                else
                {

                    TempData["ErrorMessage"] = "Error en el servidor";
                    return RedirectToAction(nameof(Index), new { id = idUsuario });
                }
            }
            catch (Exception ex)
            {

                TempData["ErrorMessage"] = "Error en el servidor";
                return RedirectToAction(nameof(Index), new { id = idUsuario });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteRol(int idUsuario, int idRol)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var permiso = new
                {
                    IdUsuario = idUsuario,
                    IdRol = idRol
                };

                var json = JsonConvert.SerializeObject(permiso);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Crear una solicitud HTTP DELETE con el contenido
                var request = new HttpRequestMessage(HttpMethod.Delete, "https://localhost:7293/api/UsuariosRoles")
                {
                    Content = content
                };

                // Hacer la solicitud
                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Rol eliminado correctamente";
                    return RedirectToAction(nameof(Index), new { id = idUsuario });

                }
                else
                {

                    TempData["ErrorMessage"] = "Error en el servidor";
                    return RedirectToAction(nameof(Index), new { id = idUsuario });
                }
            }
            catch (Exception ex)
            {

                TempData["ErrorMessage"] = "Error en el servidor";
                return RedirectToAction(nameof(Index), new { id = idUsuario });
            }
        }
    }
}
