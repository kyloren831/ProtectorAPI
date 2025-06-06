namespace ProtectorAPI.Services
{
    public class PasswordHasher
    {
        // Método para hashear la contraseña
        public static string HashPassword(string ContraseñaUsuario)
        {
            return BCrypt.Net.BCrypt.HashPassword(ContraseñaUsuario);

        }
        // Método para verificar si la contraseña es correcta
        public static bool VerifyPassword(string ContraseñaUsuario, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(ContraseñaUsuario) || string.IsNullOrEmpty(hashedPassword))
            {
                return false;
            }
            return BCrypt.Net.BCrypt.Verify(ContraseñaUsuario, hashedPassword); // Método para hashear la contraseña
        }

    }
}
