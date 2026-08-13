namespace TiendaOnline.Dominio.Configuracion;

// Guarda la configuración del correo.
public class CorreoConfiguracion
{
    // Servidor SMTP.
    public string ServidorSmtp { get; set; } = string.Empty;

    // Puerto del servidor.
    public int Puerto { get; set; }

    // Correo que envía los mensajes.
    public string CorreoRemitente { get; set; } = string.Empty;

    // Nombre visible del remitente.
    public string NombreRemitente { get; set; } = "Esencia";

    // Usuario del correo.
    public string Usuario { get; set; } = string.Empty;

    // Contraseña de aplicación.
    public string Contrasena { get; set; } = string.Empty;

    // Indica si usa TLS.
    public bool UsarTls { get; set; } = true;
}