using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using ProtectorAPP.Models.Pantallas;
using ProtectorAPP.Models.Sistemas;
using ProtectorAPP.Models.Usuarios;

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

        // GET: PantallasController/Create/1
        public async Task< ActionResult > Create(int id)
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

                        var pantalla = new PostPantallaViewModel {
                            IdSistema= sistema.IdSistema,
                            NombreSistema = sistema.Descripcion
                        };
                        return View(pantalla);
                    }
                    else
                    {
                        // Si la respuesta no es exitosa, devolver mensaje de error
                        TempData["ErrorMessage"] = "Hubo un problema al agregar el sistema.";
                        return RedirectToAction(nameof(Index), new {id=id});
                    }
                }
                else
                {
                    // Si la respuesta no es exitosa, devolver mensaje de error
                    TempData["ErrorMessage"] = "Debe iniciar sesion";
                    return RedirectToAction(nameof(Index), new { id = id });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index), new { id = id });
            }
        }

        // POST: PantallasController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PostPantallaViewModel nPantalla)
        {
            if (nPantalla.IdSistema==null || nPantalla.IdPantalla == null || nPantalla.Estado == null || nPantalla.Descripcion == null || nPantalla.ImagenFile == null)
            {
                if (nPantalla.ImagenFile == null)
                {
                    TempData["ErrorMessage"] = "Debe agregar una imagen";
                }
                TempData["ErrorMessage"] = "Debe llenar todos los datos una imagen";
                return View(nPantalla);
            }

            try
            {
                var token = HttpContext.Session.GetString("JwtToken");

                if (!string.IsNullOrEmpty(token))
                {
                    var pantalla = new PantallaViewModel
                    {
                        IdSistema = nPantalla.IdSistema,
                        IdPantalla = nPantalla.IdPantalla,
                        Descripcion = nPantalla.Descripcion,
                        Estado = nPantalla.Estado,
                        FotoUrl = ""
                    };

                    if (nPantalla.ImagenFile != null && nPantalla.ImagenFile.Length > 0)
                    {
                        // Directorio de subida
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Pantallas");

                        //  Crear carpeta si no existe
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        //  Generar un nombre único para evitar colisiones
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(nPantalla.ImagenFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        //  Guardar la imagen en el servidor
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await nPantalla.ImagenFile.CopyToAsync(fileStream);
                        }
                        //  Asignar la URL relativa para enviar a la API
                        pantalla.FotoUrl = $"/img/Pantallas/{uniqueFileName}";
                    }

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    // Llamar al API para cambiar el estado del usuario

                    var json = JsonConvert.SerializeObject(pantalla); // Serializar el objeto usuario en JSON
                    var content = new StringContent(json, Encoding.UTF8, "application/json"); // Crear el contenido de la solicitud

                    var response = await httpClient.PostAsync("Pantallas", content);
                    if (response.IsSuccessStatusCode)
                    {
                        // Al éxito, redirige y pasa el mensaje de éxito
                        TempData["SuccessMessage"] = "Pantalla agregada con exito";
                        return RedirectToAction(nameof(Index), new { id = nPantalla.IdSistema });
                    }
                    else
                    {
                        // Si la respuesta no es exitosa, devolver mensaje de error
                        TempData["ErrorMessage"] = "Hubo un problema al agregar la panta;la.";
                        return RedirectToAction(nameof(Index), new { id = nPantalla.IdSistema });
                    }
                }
                else
                {
                    // Si la respuesta no es exitosa, devolver mensaje de error
                    TempData["ErrorMessage"] = "Debe iniciar sesion";
                    return RedirectToAction(nameof(Index), new { id = nPantalla.IdSistema });
                }

            }
            catch (Exception ex)
            {
                // En caso de error, captura y muestra el mensaje
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index), new { id = nPantalla.IdSistema });
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
                    var response = await httpClient.PatchAsync($"Pantallas/{id}", null); // Aquí se puede agregar el contenido si es necesario

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonP = await response.Content.ReadAsStringAsync();
                        var pantalla = JsonConvert.DeserializeObject<PantallaViewModel>(jsonP);
                        // Al éxito, redirige y pasa el mensaje de éxito
                        TempData["SuccessMessage"] = "Estado cambiado exitosamente.";
                        return RedirectToAction(nameof(Index), new { id = pantalla.IdSistema});
                    }
                    else
                    {
                        // Si la respuesta no es exitosa, devolver mensaje de error
                        TempData["ErrorMessage"] = "Hubo un problema al cambiar el estado.";
                        return RedirectToAction(nameof(Index), new { id = 0 });
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
                return RedirectToAction(nameof(Index), new { id = id });
            }
        }
        // GET: PantallasController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JwtToken");
                if (!string.IsNullOrEmpty(token))
                {

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var response = await httpClient.GetAsync($"Pantallas/{id}"); // Aquí se puede agregar el contenido si es necesario

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        var pantalla = JsonConvert.DeserializeObject<PantallaViewModel>(json);

                        if (pantalla != null)
                        {
                            var responseSys = await httpClient.GetAsync($"Sistemas/Buscar?id={pantalla.IdSistema}");
                            if (responseSys.IsSuccessStatusCode)
                            {
                                var jsonSys = await response.Content.ReadAsStringAsync();
                                var sistema = JsonConvert.DeserializeObject<SistemasViewModel>(jsonSys);

                                var pantallaVM = new PutPantallaViewModel
                                {
                                    IdSistema = sistema.IdSistema,
                                    NombreSistema = sistema.Descripcion,
                                    IdPantalla = pantalla.IdPantalla,
                                    Descripcion = pantalla.Descripcion,
                                    Estado = pantalla.Estado,
                                    FotoUrl = pantalla.FotoUrl
                                };

                                return View(pantallaVM);
                            }
                            else
                            {
                                // Si la respuesta no es exitosa, devolver mensaje de error
                                TempData["ErrorMessage"] = "Hubo un problema al agregar el sistema.";
                                return RedirectToAction(nameof(Index), new { id = id });
                            }
                            return View(pantalla);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "usuario null";
                            return RedirectToAction(nameof(Index), new { id = id });
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error en la respuesta al buscar usuario";
                        return RedirectToAction(nameof(Index), new { id = id });
                    }
                }
                else
                {
                    return RedirectToAction("Login", "Auth");
                }
            }
            catch (Exception ex)
            {

                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index), new { id = id });
            }
        }

        // POST: PantallasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, PutPantallaViewModel putPantallaViewModel)
        {
            if (putPantallaViewModel.IdSistema == null || putPantallaViewModel.IdPantalla == null || putPantallaViewModel.Estado == null || putPantallaViewModel.Descripcion == null || putPantallaViewModel.ImagenFile == null)
            {
                if (putPantallaViewModel.ImagenFile == null)
                {
                    TempData["ErrorMessage"] = "Debe agregar una imagen";
                }
                TempData["ErrorMessage"] = "Debe llenar todos los datos una imagen";
                return View(putPantallaViewModel);
            }

            if (putPantallaViewModel.ImagenFile != null && putPantallaViewModel.ImagenFile.Length > 0)
            {
                // Directorio de subida
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Pantallas");

                //  Crear carpeta si no existe
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                //  Generar un nombre único para evitar colisiones
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(putPantallaViewModel.ImagenFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                //  Guardar la imagen en el servidor
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await putPantallaViewModel.ImagenFile.CopyToAsync(fileStream);
                }
                //  Asignar la URL relativa para enviar a la API
                putPantallaViewModel.FotoUrl = $"/img/Pantallas/{uniqueFileName}";
            }

            try
            {
                var token = HttpContext.Session.GetString("JwtToken");

                if (string.IsNullOrEmpty(token))
                    return RedirectToAction("Login", "Auth");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(putPantallaViewModel); // Serializar el objeto usuario en JSON
                var content = new StringContent(json, Encoding.UTF8, "application/json"); // Crear el contenido de la solicitud

                var response = await httpClient.PutAsync($"Pantallas/{putPantallaViewModel.IdPantalla}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Pantalla editada con exito";

                    return RedirectToAction(nameof(Index), new { id = putPantallaViewModel.IdSistema});
                }
                else
                {
                    TempData["ErrorMessage"] = "Error en la respuesta";
                    return View(putPantallaViewModel);
                }


            }
            catch
            {

                TempData["ErrorMessage"] = "Error encontrando al usuario";
                return View(putPantallaViewModel);
            }
        }
        public async Task<ActionResult> Listar()
        {
            try
            {
                var token = HttpContext.Session.GetString("JwtToken");

                if (string.IsNullOrEmpty(token))
                    return RedirectToAction("Login", "Auth");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await httpClient.GetAsync($"Pantallas");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var pantallas = JsonConvert.DeserializeObject<List<PantallaViewModel>>(json);
                    return Json(pantallas); // Retorna la lista de pantallas en formato JSON
                }
                else
                {
                    TempData["ErrorMessage"] = "Error en la respuesta";
                    return Json(new { success = false, message = "Error en la respuesta del servidor." });
                }
            }
            catch
            {
                TempData["ErrorMessage"] = "Error encontrando al usuario";
                return Json(new { success = false, message = "Error interno al procesar la solicitud." });
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
