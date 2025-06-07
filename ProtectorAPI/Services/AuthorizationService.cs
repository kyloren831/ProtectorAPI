using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProtectorAPI.Data;
using ProtectorAPI.Models;

namespace ProtectorAPI.Services
{
    public interface IAuthorizationService
    {
        Task<AuthorizationResponse> GetToken(string email, string password);
    }

    public class AuthorizationService : IAuthorizationService
    {
        private readonly IConfiguration configuration;
        private readonly ProtectorDbContext _context;
        private readonly IUserService _userService;

        public AuthorizationService(IConfiguration configuration, ProtectorDbContext context, IUserService userService)
        {
            this.configuration = configuration;
            this._context = context;
            this._userService = userService;
        }

        public async Task<AuthorizationResponse> GetToken(string email, string password)
        {
            var temp = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == email && u.Estado == 'A');

            if (temp == null)
                return null;

            var hasher = new PasswordHasher<Usuario>();
            var result = hasher.VerifyHashedPassword(temp, temp.Contrasenna, password);

            if (result != PasswordVerificationResult.Success)
                return null;

            // Obtener las pantallas y permisos filtrados
            var pantallasConPermisos = await _userService.ObtenerPantallasConPermisos(temp.IdUsuario);

            // Serializar el JSON
            var pantallasJson = JsonSerializer.Serialize(pantallasConPermisos);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, temp.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, temp.Nombre),
                new Claim("pantallas", pantallasJson)
            };

            // Generar el token
            var keyBytes = Encoding.UTF8.GetBytes(configuration["JwtSettings:key"]);
            var key = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            //Retornar la respuesta con el token y datos básicos del usuario
            return new AuthorizationResponse
            {
                Token = jwt,
                Nombre = temp.Nombre,
                Correo = temp.Correo
            };
        }
    }
}
