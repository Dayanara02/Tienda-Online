// Importa la entidad Notificacion.
using TiendaOnline.Dominio.Entidades;

// Importa la clase Respuesta.
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Define las operaciones de notificaciones.
    public interface INotificacionLN
    {
        // Permite registrar una notificación.
        Task<Respuesta<Notificacion>>
            InsertarAsync(
                Notificacion datos
            );

        // Permite listar las notificaciones.
        Task<Respuesta<IEnumerable<Notificacion>>>
            ListarAsync();

        // Permite modificar una notificación.
        Task<Respuesta<Notificacion>>
            ModificarAsync(
                Notificacion datos
            );

        // Permite eliminar una notificación.
        Task<Respuesta<bool>>
            EliminarAsync(
                Notificacion datos
            );

        // Permite buscar notificaciones.
        Task<Respuesta<IEnumerable<Notificacion>>>
            BuscarAsync(
                Notificacion datos
            );

        // Permite obtener una notificación.
        Task<Respuesta<Notificacion>>
            ObtenerAsync(
                Notificacion datos
            );
    }
}