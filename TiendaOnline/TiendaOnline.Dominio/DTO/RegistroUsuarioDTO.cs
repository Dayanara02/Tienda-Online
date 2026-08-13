namespace TiendaOnline.Dominio.DTO;

// DTO utilizado para recibir los datos necesarios para registrar un nuevo usuario.
public class RegistroUsuarioDto
{
    // Nombre del usuario.
    public string Nombre { get; set; } = string.Empty;

    // Apellido del usuario.
    public string Apellido { get; set; } = string.Empty;

    // Correo electrónico del usuario.
    public string Correo { get; set; } = string.Empty;

    // Contraseña que utilizará el usuario para iniciar sesión.
    public string Contrasena { get; set; } = string.Empty;

    // Número de teléfono del usuario. Puede quedar vacío.
    public string? Telefono { get; set; }
}