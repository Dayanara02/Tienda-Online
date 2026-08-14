// Importa los DTO utilizados en autenticación.
using TiendaOnline.Dominio.DTO;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Define las operaciones principales de autenticación.
    public interface IAuthServicio
    {
        // Permite iniciar sesión.
        Task<RespuestaLoginDto?> LoginAsync(
            LoginDto login
        );

        // Permite registrar un nuevo usuario.
        Task<bool> RegistrarAsync(
            RegistroUsuarioDto registro
        );
    }
}