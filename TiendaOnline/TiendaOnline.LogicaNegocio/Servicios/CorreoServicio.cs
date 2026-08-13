// Permite definir la seguridad de la conexión.
using MailKit.Security;

// Permite leer la configuración del correo.
using Microsoft.Extensions.Options;

// Permite crear el mensaje y adjuntar archivos.
using MimeKit;

// Importa la configuración del correo.
using TiendaOnline.Dominio.Configuracion;

// Importa la interfaz del servicio.
using TiendaOnline.Dominio.InterfacesLN;

namespace TiendaOnline.LogicaNegocio.Servicios;

// Envía correos desde la tienda.
public class CorreoServicio : ICorreoServicio
{
    // Guarda la configuración del correo.
    private readonly CorreoConfiguracion _configuracion;

    // Recibe la configuración del appsettings.
    public CorreoServicio(
        IOptions<CorreoConfiguracion> configuracion)
    {
        // Guarda la configuración.
        _configuracion = configuracion.Value;
    }

    // Envía el comprobante PDF.
    public async Task EnviarComprobanteAsync(
        string destinatario,
        string nombreCliente,
        int idPedido,
        byte[] archivoPdf)
    {
        // Crea el mensaje.
        var mensaje = new MimeMessage();

        // Agrega el remitente.
        mensaje.From.Add(
            new MailboxAddress(
                _configuracion.NombreRemitente,
                _configuracion.CorreoRemitente
            )
        );

        // Agrega el destinatario.
        mensaje.To.Add(
            MailboxAddress.Parse(
                destinatario
            )
        );

        // Define el asunto.
        mensaje.Subject =
            $"Comprobante de pago - Pedido #{idPedido}";

        // Crea el cuerpo del correo.
        var cuerpo = new BodyBuilder();

        // Agrega el mensaje.
        cuerpo.HtmlBody =
            $"""
            <div style="font-family: Arial, sans-serif;">
                <h2 style="color: #1f4e79;">
                    Esencia
                </h2>

                <p>
                    Hola {nombreCliente},
                </p>

                <p>
                    Tu pago del pedido
                    <strong>#{idPedido}</strong>
                    fue realizado correctamente.
                </p>

                <p>
                    Adjuntamos tu comprobante
                    en formato PDF.
                </p>

                <p>
                    Gracias por comprar en Esencia.
                </p>
            </div>
            """;

        // Adjunta el PDF.
        cuerpo.Attachments.Add(
            $"Comprobante_Pedido_{idPedido}.pdf",
            archivoPdf,
            new ContentType(
                "application",
                "pdf"
            )
        );

        // Asigna el contenido.
        mensaje.Body =
            cuerpo.ToMessageBody();

        // Crea el cliente SMTP de MailKit.
        using var cliente =
            new MailKit.Net.Smtp.SmtpClient();

        // Define la seguridad.
        var seguridad =
            _configuracion.UsarTls
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.None;

        // Conecta con el servidor SMTP.
        await cliente.ConnectAsync(
            _configuracion.ServidorSmtp,
            _configuracion.Puerto,
            seguridad
        );

        // Inicia sesión.
        await cliente.AuthenticateAsync(
            _configuracion.Usuario,
            _configuracion.Contrasena
        );

        // Envía el mensaje.
        await cliente.SendAsync(
            mensaje
        );

        // Cierra la conexión.
        await cliente.DisconnectAsync(
            true
        );
    }
}