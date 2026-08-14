namespace TiendaOnline.Dominio.DTO;

// DTO utilizado para recibir los datos necesarios para iniciar sesión.
public class LoginDto
{
    // Correo electrónico utilizado por el usuario para iniciar sesión.
    public string Correo { get; set; } = string.Empty;

    // Contraseña utilizada por el usuario para iniciar sesión.
    public string Contrasena { get; set; } = string.Empty;
}