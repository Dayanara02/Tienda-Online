namespace TiendaOnline.Dominio.DTO;

public class RespuestaLoginDto
{
    public int IdUsuario { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public DateTime Expiracion { get; set; }
}