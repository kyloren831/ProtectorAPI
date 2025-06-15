using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtectorAPP.Models;
using System.Text;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Common;

namespace ProtectorAPP.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly HttpClient _httpClient;
        public UsuariosController(IHttpClientFactory httpClient)
        {
            this._httpClient = httpClient.CreateClient("ApiClient");
        }
        // GET: UsuariosController
        public async Task< ActionResult >Index()
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var usuarios = new List<UsuarioViewModel>();
            try
            {
                var response = await _httpClient.GetAsync("Usuarios");
                if (response.IsSuccessStatusCode) { 
                    var json = await response.Content.ReadAsStringAsync();
                    usuarios = JsonConvert.DeserializeObject<List<UsuarioViewModel>>(json);
                }
                else
                {
                    ViewData["Error"] = "Error en el servidor";
                }
            }
            catch (Exception ex) {
                ViewData["Error"] = "Error en el servidor";
            }
            return View(usuarios);
        }

        // GET: UsuariosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsuariosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuariosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PostUsuarioViewModel usuarioViewModel)
        {
            if (!ModelState.IsValid || usuarioViewModel.ImagenFile == null)
            {
                if (usuarioViewModel.ImagenFile == null)
                {
                    TempData["ErrorMessage"] = "Debe agregar una imagen";
                }
                return View(usuarioViewModel);
            }
                
            try
            {
                var token = HttpContext.Session.GetString("JwtToken");

                if (!string.IsNullOrEmpty(token))
                {
                    var usuario = new PostUsuario { 
                        Nombre = usuarioViewModel.Nombre,
                        Correo = usuarioViewModel.Correo,
                        FotoUrl = ""
                    };

                    if (usuarioViewModel.ImagenFile != null && usuarioViewModel.ImagenFile.Length > 0)
                    {
                        // Directorio de subida
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Usuarios");

                        //  Crear carpeta si no existe
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        //  Generar un nombre único para evitar colisiones
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(usuarioViewModel.ImagenFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        //  Guardar la imagen en el servidor
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await usuarioViewModel.ImagenFile.CopyToAsync(fileStream);
                        }
                        //  Asignar la URL relativa para enviar a la API
                        usuario.FotoUrl = $"/img/Usuarios/{uniqueFileName}";
                    }

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    // Llamar al API para cambiar el estado del usuario

                    var json = JsonConvert.SerializeObject(usuario); // Serializar el objeto usuario en JSON
                    var content = new StringContent(json, Encoding.UTF8, "application/json"); // Crear el contenido de la solicitud

                    var response = await _httpClient.PostAsync("Usuarios", content);
                    if (response.IsSuccessStatusCode)
                    {
                        // Al éxito, redirige y pasa el mensaje de éxito
                        TempData["SuccessMessage"] = "Usuario agregado con exito";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        // Si la respuesta no es exitosa, devolver mensaje de error
                        TempData["ErrorMessage"] = "Hubo un problema al agregar al Usuario.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    // Si la respuesta no es exitosa, devolver mensaje de error
                    TempData["ErrorMessage"] = "Debe iniciar sesion";
                    return RedirectToAction(nameof(Index));
                }

            }
            catch (Exception ex)
            {
                // En caso de error, captura y muestra el mensaje
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: UsuariosController/Edit/5
        public async Task< ActionResult > Edit(int id)
        {
            
            try
            {
                var token = HttpContext.Session.GetString("JwtToken");
                if (!string.IsNullOrEmpty(token))
                {

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await _httpClient.GetAsync($"Usuarios/{id}"); // Aquí se puede agregar el contenido si es necesario

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        var user = JsonConvert.DeserializeObject<PutUsuarioViewModel>(json);

                        if (user != null) {
                            return View(user);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "usuario null";
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error en la respuesta al buscar usuario";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Auth");
                }
            }
            catch (Exception ex) {
                TempData["ErrorMessage"] = "Error encontrando al usuario";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: UsuariosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PutUsuarioViewModel putUsuarioViewModel)
        {
            if (string.IsNullOrEmpty(putUsuarioViewModel.Correo) || string.IsNullOrEmpty(putUsuarioViewModel.Nombre))
            {
                TempData["ErrorMessage"] = "Debe llenar todos los campos";
                return View(putUsuarioViewModel);
            }

            if (putUsuarioViewModel.ImagenFile != null && putUsuarioViewModel.ImagenFile.Length > 0)
            {
                // Directorio de subida
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Usuarios");

                //  Crear carpeta si no existe
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                //  Generar un nombre único para evitar colisiones
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(putUsuarioViewModel.ImagenFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                //  Guardar la imagen en el servidor
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await putUsuarioViewModel.ImagenFile.CopyToAsync(fileStream);
                }
                //  Asignar la URL relativa para enviar a la API
                putUsuarioViewModel.FotoUrl = $"/img/Usuarios/{uniqueFileName}";
            }

            try
            {
                var token = HttpContext.Session.GetString("JwtToken");

                if (string.IsNullOrEmpty(token))
                     return RedirectToAction("Login", "Auth");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(putUsuarioViewModel); // Serializar el objeto usuario en JSON
                var content = new StringContent(json, Encoding.UTF8, "application/json"); // Crear el contenido de la solicitud

                var response = await _httpClient.PutAsync($"Usuarios/{putUsuarioViewModel.IdUsuario}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Usuario editado con exito";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Error en la respuesta";
                    return View(putUsuarioViewModel);
                }


            }
            catch
            {

                TempData["ErrorMessage"] = "Error encontrando al usuario";
                return View(putUsuarioViewModel);
            }
        }

        // GET: UsuariosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // PATCH: UsuariosController/5
        // PATCH: UsuariosController/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CambiarEstado(int id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JwtToken");

                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    // Llamar al API para cambiar el estado del usuario
                    var response = await _httpClient.PatchAsync($"Usuarios/{id}", null); // Aquí se puede agregar el contenido si es necesario

                    if (response.IsSuccessStatusCode)
                    {
                        // Al éxito, redirige y pasa el mensaje de éxito
                        TempData["SuccessMessage"] = "Estado cambiado exitosamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        // Si la respuesta no es exitosa, devolver mensaje de error
                        TempData["ErrorMessage"] = "Hubo un problema al cambiar el estado.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Auth");
                }
               
            }
            catch (Exception ex)
            {
                // En caso de error, captura y muestra el mensaje
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

    }
}
