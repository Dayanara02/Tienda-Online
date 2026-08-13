
namespace TiendaOnline.Dominio.DTO;

// Representa una dirección de un usuario.
public class DireccionUsuarioDto
{
    // Identificador de la dirección.
    public int IdDireccion { get; set; }

    // Usuario propietario.
    public int IdUsuario { get; set; }

    // Provincia.
    public string Provincia { get; set; } =
        string.Empty;

    // Cantón.
    public string Canton { get; set; } =
        string.Empty;

    // Distrito.
    public string Distrito { get; set; } =
        string.Empty;

    // Dirección exacta.
    public string DireccionExacta { get; set; } =
        string.Empty;

    // Código postal.
    public string? CodigoPostal { get; set; }

    // Indica si es la dirección principal.
    public bool Principal { get; set; }

    // Indica si está activa.
    public bool Estado { get; set; } = true;
}