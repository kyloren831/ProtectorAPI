using System.Security.Cryptography;
using System.Text;

namespace ProtectorAPI.Services
{
    public class PasswordGenerator
    {
        public static string Generar(int longitud)
        {
            const string caracteresPermitidos = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            var bytesAleatorios = new byte[longitud];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytesAleatorios);
            }

            var sb = new StringBuilder(longitud);

            foreach (var b in bytesAleatorios)
            {
                var idx = b % caracteresPermitidos.Length;
                sb.Append(caracteresPermitidos[idx]);
            }

            return sb.ToString();
        }
    }
}
