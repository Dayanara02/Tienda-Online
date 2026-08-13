using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Esta interfaz define las operaciones de la lista de deseos.
    public interface IListaDeseoLN
    {
        // Permite registrar una nueva lista de deseos.
        Task<Respuesta<ListaDeseo>> InsertarAsync(ListaDeseo datos);

        // Permite listar todas las listas de deseos registradas.
        Task<Respuesta<IEnumerable<ListaDeseo>>> ListarAsync();

        // Permite modificar la información de una lista de deseos existente.
        Task<Respuesta<ListaDeseo>> ModificarAsync(ListaDeseo datos);

        // Permite eliminar una lista de deseos existente.
        Task<Respuesta<bool>> EliminarAsync(ListaDeseo datos);

        // Permite buscar listas de deseos utilizando los datos recibidos.
        Task<Respuesta<IEnumerable<ListaDeseo>>> BuscarAsync(ListaDeseo datos);

        // Permite obtener una lista de deseos específica por su identificador.
        Task<Respuesta<ListaDeseo>> ObtenerAsync(ListaDeseo datos);
    }
}
