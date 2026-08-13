// Importa las entidades necesarias.
using TiendaOnline.Dominio.Entidades;

// Importa la clase Respuesta.
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Define las operaciones principales de autenticación.
    public interface IAuthServicio
    {
        // Permite iniciar sesión con correo y contraseña.
        Task<Respuesta<Usuario>>
            IniciarSesionAsync(
                string correo,
                string contrasena
            );

        // Permite registrar un nuevo usuario.
        Task<Respuesta<Usuario>>
            RegistrarAsync(
                Usuario usuario
            );

        // Permite obtener un usuario por correo.
        Task<Respuesta<Usuario>>
            ObtenerPorCorreoAsync(
                string correo
            );
    }
}