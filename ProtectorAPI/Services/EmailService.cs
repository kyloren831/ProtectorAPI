using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace ProtectorAPI.Services
{
    public interface IEmailService
    {
        Task EnviarEmail(string emailReceptor, string tema, string cuerpo);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration config)
        {
            this.configuration = config;
        }

        public async Task EnviarEmail(string emailReceptor, string tema, string cuerpo)
        {
            // Obtiene el email del remitente desde la configuración
            var emailEmisor = configuration.GetValue<string>("CONFIGURACIONES_EMAIL:EMAIL");

            // Obtiene la contraseña del email del remitente
            var password = configuration.GetValue<string>("CONFIGURACIONES_EMAIL:PASSWORD");

            // Obtiene el host del servidor SMTP (ej. smtp.gmail.com)
            var host = configuration.GetValue<string>("CONFIGURACIONES_EMAIL:HOST");

            // Obtiene el puerto SMTP (ej. 587 para TLS)
            var port = configuration.GetValue<int>("CONFIGURACIONES_EMAIL:PORT");

            // Crea un cliente SMTP usando el host y el puerto configurados
            using (var smtpCliente = new SmtpClient(host, port))
            {
                smtpCliente.EnableSsl = true;
                smtpCliente.UseDefaultCredentials = false;
                smtpCliente.Credentials = new NetworkCredential(emailEmisor, password);

                using (var mensaje = new MailMessage(emailEmisor, emailReceptor, tema, cuerpo))
                {
                    mensaje.IsBodyHtml = true;
                    await smtpCliente.SendMailAsync(mensaje);
                }
            }
        }
    }
}
