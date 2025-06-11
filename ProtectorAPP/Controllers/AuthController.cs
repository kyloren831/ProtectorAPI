using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtectorAPP.Models;

namespace ProtectorAPP.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient httpClient;

        public AuthController(IHttpClientFactory httpClient)
        {
            this.httpClient = httpClient.CreateClient("ApiClient"); ;
        }


        private bool SonPermisosIguales(List<PantallaConPermisosDTO> permisosTotales, List<PantallaConPermisosDTO> permisosUsuario)
        {
            // Verificar si las listas tienen el mismo número de pantallas
            if (permisosTotales.Count != permisosUsuario.Count)
            {
                return false;
            }

            // Comparar cada pantalla con sus permisos correspondientes
            foreach (var pantallaTotal in permisosTotales)
            {
                var pantallaUsuario = permisosUsuario.FirstOrDefault(p => p.IdPantalla == pantallaTotal.IdPantalla);

                // Si no se encuentra la pantalla o los permisos no coinciden
                if (pantallaUsuario == null)
                {
                    return false;
                }

                // Obtener solo los IdPermiso de cada lista
                var permisosTotalesIds = pantallaTotal.Permisos.Select(p => p.IdPermiso).ToList();
                var permisosUsuarioIds = pantallaUsuario.Permisos.Select(p => p.IdPermiso).ToList();

                // Comparar las listas de IdPermiso sin importar el orden
                if (!permisosTotalesIds.All(ptId => permisosUsuarioIds.Contains(ptId)) ||
                    !permisosUsuarioIds.All(puId => permisosTotalesIds.Contains(puId)))
                {
                    return false;
                }
            }

            // Si todas las pantallas y permisos coinciden
            return true;
        }

        // GET: AuthController
        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Si está autenticado, redirigir al home o alguna página principal
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Debe ingresar correo y contraseña";
                return View();
            }

            var loginRequest = new
            {
                email = email,
                password = password
            };

            var response = await httpClient.PostAsJsonAsync("Usuarios/login", loginRequest);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var authResponse = JsonConvert.DeserializeObject<AutorizacionResponse>(json);

                if (authResponse != null)
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(authResponse.Token);

                    var userId = jwt?.Claims?.FirstOrDefault(c => c.Type == "nameid")?.Value;
                    var userName = jwt?.Claims?.FirstOrDefault(c => c.Type == "unique_name")?.Value;
                   

                    // Extraer los permisos del claim 'pantallas' del token
                    var pantallasJson = jwt?.Claims?.FirstOrDefault(c => c.Type == "pantallas")?.Value;
                    var permisosUsuario = string.IsNullOrEmpty(pantallasJson)
                                        ? new List<PantallaConPermisosDTO>()
                                        : JsonConvert.DeserializeObject<List<PantallaConPermisosDTO>>(pantallasJson);

                    // Obtener los permisos totales desde el API
             
                    var totalPermisosResponse = await httpClient.GetAsync("Permisos/TotalPermisos");

                    if (totalPermisosResponse.IsSuccessStatusCode)
                    {
                        var permisosTotalesJSON = await totalPermisosResponse.Content.ReadAsStringAsync();

                        var permisosTotales = string.IsNullOrEmpty(permisosTotalesJSON)
                                            ? new List<PantallaConPermisosDTO>()
                                            : JsonConvert.DeserializeObject<List<PantallaConPermisosDTO>>(permisosTotalesJSON);

                        // Verificar si las dos listas de permisos son iguales
                        if (SonPermisosIguales(permisosTotales, permisosUsuario))
                        {
                            // Asignar el rol de admin si las listas de permisos son iguales
                            // Crear los claims para la cookie de autenticación
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, userName),            // Nombre del usuario
                                new Claim(ClaimTypes.Email, email),              // Email del usuario
                                new Claim("UserId", userId),                     // ID del usuario
                                new Claim(ClaimTypes.Role, "Admin")               // Rol por defecto, puede ser cambiado a Admin
                            };

                            // Crear la identidad de claims
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            // Propiedades de autenticación
                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = false,                             // Persistente (la cookie durará entre sesiones)
                                ExpiresUtc = DateTime.UtcNow.AddMinutes(30)      // Expiración de la cookie (30 minutos)
                            };

                            // Eliminar cualquier sesión anterior y crear una nueva con cookies
                            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                        }
                        else
                        {
                            // Crear los claims para la cookie de autenticación
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, userName),            // Nombre del usuario
                                new Claim(ClaimTypes.Email, email),              // Email del usuario
                                new Claim("UserId", userId),                     // ID del usuario
                                new Claim(ClaimTypes.Role, "User")               // Rol por defecto, puede ser cambiado a Admin
                            };

                            // Crear la identidad de claims
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            // Propiedades de autenticación
                            var authProperties = new AuthenticationProperties
                            {
                                IsPersistent = false,                             // Persistente (la cookie durará entre sesiones)
                                ExpiresUtc = DateTime.UtcNow.AddMinutes(30)      // Expiración de la cookie (30 minutos)
                            };

                            // Eliminar cualquier sesión anterior y crear una nueva con cookies
                            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                        }

                        // Redirigir a la página principal
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Error = "Error al obtener los permisos del sistema";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Error = "Error al autenticar usuario";
                    return View();
                }
            }
            else
            {
                ViewBag.Error = "Error al autenticar usuario";
                return View();
            }
        }



        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Eliminar la cookie
            TempData["Mensaje"] = "Sesión cerrada correctamente.";
            return RedirectToAction("Login");
        }

        // GET: AuthController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AuthController/Create
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

        // GET: AuthController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AuthController/Edit/5
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

        // GET: AuthController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AuthController/Delete/5
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
