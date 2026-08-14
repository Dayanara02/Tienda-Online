
namespace TiendaOnline.Dominio.DTO;

// Guarda los datos necesarios para crear una notificación.
public class NotificacionCrearDto
{
    // Usuario que recibirá la notificación.
    public int IdUsuario { get; set; }

    // Título de la notificación.
    public string Titulo { get; set; } =
        string.Empty;

    // Mensaje que recibirá el usuario.
    public string Mensaje { get; set; } =
        string.Empty;

    // Tipo de notificación.
    public string? Tipo { get; set; }
}