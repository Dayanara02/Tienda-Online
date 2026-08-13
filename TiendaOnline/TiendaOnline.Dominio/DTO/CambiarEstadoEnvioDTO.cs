
namespace TiendaOnline.Dominio.DTO;

// Guarda el nuevo estado de un envío.
public class CambiarEstadoEnvioDto
{
    // Nuevo estado del envío.
    public string Estado { get; set; } =
        string.Empty;
}