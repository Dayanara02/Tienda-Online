// Define el espacio de nombres de las entidades tipadas.
namespace TiendaOnline.Dominio.EntidadesTipadas;

// Representa los datos tipados de un usuario.
public class TUsuario
{
    // Identificador del usuario.
    public int IdUsuario { get; set; }

    // Identificador del rol.
    public int IdRol { get; set; }

    // Nombre del usuario.
    public string Nombre { get; set; }

    // Apellido del usuario.
    public string Apellido { get; set; }

    // Correo electrónico.
    public string Correo { get; set; }

    // Contraseña del usuario.
    public string Contrasena { get; set; }

    // Número de teléfono.
    public string Telefono { get; set; }

    // Estado del usuario.
    public bool Estado { get; set; }
}