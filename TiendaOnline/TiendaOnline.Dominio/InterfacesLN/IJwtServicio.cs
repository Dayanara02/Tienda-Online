// Importa la entidad Usuario.
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.Dominio.InterfacesLN;

// Define las funciones relacionadas con JWT.
public interface IJwtServicio
{
    // Genera el token del usuario autenticado.
    string GenerarToken(
        Usuario usuario,
        string nombreRol,
        DateTime fechaExpiracion
    );
}