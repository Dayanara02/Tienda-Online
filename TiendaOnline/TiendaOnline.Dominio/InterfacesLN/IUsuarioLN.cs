using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Esta interfaz define las operaciones de usuarios en el sistema.
    public interface IUsuarioLN
    {
        // Registra un nuevo usuario en el sistema.
        Task<Respuesta<Usuario>> InsertarAsync(Usuario datos);

        // Obtiene todos los usuarios registrados.
        Task<Respuesta<IEnumerable<Usuario>>> ListarAsync();

        // Modifica la información de un usuario existente.
        Task<Respuesta<Usuario>> ModificarAsync(Usuario datos);

        // Elimina un usuario utilizando su identificador.
        Task<Respuesta<bool>> EliminarAsync(Usuario datos);

        // Busca usuarios utilizando los datos recibidos.
        Task<Respuesta<IEnumerable<Usuario>>> BuscarAsync(Usuario datos);

        // Obtiene un usuario específico utilizando su IdUsuario.
        Task<Respuesta<Usuario>> ObtenerAsync(Usuario datos);
    }
}