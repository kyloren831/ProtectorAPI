using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtectorAPP.Models.Sistemas;

namespace ProtectorAPP.Controllers
{
    public class SistemasController : Controller
    {
        private readonly HttpClient httpClient;

        public SistemasController(IHttpClientFactory httpClient)
        {
            this.httpClient = httpClient.CreateClient("ApiClient");
        }

        // GET: SistemasController
        public async Task< ActionResult > Index()
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var sistemas = new List<SistemasViewModel>();
            try
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await httpClient.GetAsync("Sistemas/Listar");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    sistemas = JsonConvert.DeserializeObject<List<SistemasViewModel>>(json);
                }
                else
                {
                    ViewData["Error"] = "Error en el servidor";
                }
            }
            catch (Exception ex)
            {
                ViewData["Error"] = "Error en el servidor";
            }
            return View(sistemas);
        }


        // GET: SistemasController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SistemasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SistemasViewModel nSistema)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Debe llenar todos los espacios";
                return View(nSistema);
            }

            try
            {
                var token = HttpContext.Session.GetString("JwtToken");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    // Llamar al API para cambiar el estado del usuario

                    var json = JsonConvert.SerializeObject(nSistema); // Serializar el objeto usuario en JSON
                    var content = new StringContent(json, Encoding.UTF8, "application/json"); // Crear el contenido de la solicitud

                    var response = await httpClient.PostAsync("Sistemas/Guardar", content);
                    if (response.IsSuccessStatusCode)
                    {
                        // Al éxito, redirige y pasa el mensaje de éxito
                        TempData["SuccessMessage"] = "Sistema agregado con exito";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        // Si la respuesta no es exitosa, devolver mensaje de error
                        TempData["ErrorMessage"] = "Hubo un problema al agregar el sistema.";
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

        // GET: SistemasController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JwtToken");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    // Llamar al API para cambiar el estado del usuario
                

                    var response = await httpClient.GetAsync($"Sistemas/Buscar?id={id}");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var sistema = JsonConvert.DeserializeObject<SistemasViewModel>(json);
                        return View(sistema);
                    }
                    else
                    {
                        // Si la respuesta no es exitosa, devolver mensaje de error
                        TempData["ErrorMessage"] = "Hubo un problema al agregar el sistema.";
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
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: SistemasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SistemasViewModel sistema)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Debe llenar todos los espacios";
                return View(sistema);
            }

            try
            {
                var token = HttpContext.Session.GetString("JwtToken");

                if (string.IsNullOrEmpty(token))
                    return RedirectToAction("Login", "Auth");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(sistema); // Serializar el objeto usuario en JSON
                var content = new StringContent(json, Encoding.UTF8, "application/json"); // Crear el contenido de la solicitud

                var response = await httpClient.PutAsync($"Sistemas/Actualizar?id={sistema.IdSistema}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Sistema editado con exito";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Error en la respuesta";
                    return View(sistema);
                }


            }
            catch
            {

                TempData["ErrorMessage"] = "Error encontrando el sistema";
                return View(sistema);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CambiarEstado(int id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JwtToken");

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    // Llamar al API para cambiar el estado del usuario
                    var response = await httpClient.PatchAsync($"Sistemas/{id}", null); // Aquí se puede agregar el contenido si es necesario

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
