namespace TiendaOnline.Dominio.DTO;

// DTO que contiene la información devuelta cuando un usuario inicia sesión correctamente.
public class RespuestaLoginDto
{
    // Identificador único del usuario.
    public int IdUsuario { get; set; }

    // Nombre completo del usuario.
    public string NombreCompleto { get; set; } = string.Empty;

    // Correo electrónico del usuario.
    public string Correo { get; set; } = string.Empty;

    // Rol o tipo de usuario dentro del sistema.
    public string Rol { get; set; } = string.Empty;

    // Token utilizado para autenticar las solicitudes del usuario.
    public string Token { get; set; } = string.Empty;

    // Fecha y hora en que el token dejará de ser válido.
    public DateTime Expiracion { get; set; }
}