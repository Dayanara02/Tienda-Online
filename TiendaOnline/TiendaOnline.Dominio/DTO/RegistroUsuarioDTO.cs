namespace TiendaOnline.Dominio.DTO;

public class RegistroUsuarioDto
{
    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string Contrasena { get; set; } = string.Empty;

    public string? Telefono { get; set; }
}