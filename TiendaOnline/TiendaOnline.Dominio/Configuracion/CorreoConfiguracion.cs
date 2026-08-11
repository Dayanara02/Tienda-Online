
namespace TiendaOnline.Dominio.Configuracion;

public class CorreoConfiguracion
{
    // Dirección del servidor SMTP.
    //
    // Por ejemplo, para Gmail se utiliza:
    // smtp.gmail.com
    public string ServidorSmtp { get; set; } = string.Empty;


    // Puerto utilizado por el servidor SMTP.
    //
    // Normalmente se utiliza 587 con TLS.
    public int Puerto { get; set; }


    // Correo desde el cual Esencia
    // enviará los mensajes.
    public string CorreoRemitente { get; set; } = string.Empty;


    // Nombre que verá el Cliente
    // como remitente del correo.
    public string NombreRemitente { get; set; } = "Esencia";


    // Usuario utilizado para iniciar sesión
    // en el servidor de correo.
    //
    // Generalmente será el mismo correo remitente.
    public string Usuario { get; set; } = string.Empty;


    // Contraseña utilizada para autenticarse.
    //
    // En Gmail debe utilizarse una contraseña
    // de aplicación y no la contraseña normal.
    public string Contrasena { get; set; } = string.Empty;


    // Indica si la conexión utilizará TLS.
    public bool UsarTls { get; set; } = true;
}