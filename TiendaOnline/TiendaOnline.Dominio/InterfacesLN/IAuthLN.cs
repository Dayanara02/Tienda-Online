// Importa la entidad Usuario.
using TiendaOnline.Dominio.Entidades;

// Importa la clase Respuesta.
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Define las operaciones de autenticación.
    public interface IAuthLN
    {
        // Permite iniciar sesión.
        Task<Respuesta<Usuario>>
            IniciarSesionAsync(
                string correo,
                string contrasena
            );

        // Permite registrar un usuario.
        Task<Respuesta<Usuario>>
            RegistrarAsync(
                Usuario usuario
            );

        // Permite buscar un usuario por correo.
        Task<Respuesta<Usuario>>
            ObtenerPorCorreoAsync(
                string correo
            );

        // Comprueba si un correo ya está registrado.
        Task<Respuesta<bool>>
            ExisteCorreoAsync(
                string correo
            );
    }
}