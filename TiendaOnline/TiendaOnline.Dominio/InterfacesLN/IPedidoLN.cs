using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Esta interfaz define las operaciones de pedidos.
    public interface IPedidoLN
    {
        // Permite registrar un nuevo pedido.
        Task<Respuesta<Pedido>> InsertarAsync(Pedido datos);

        // Permite obtener todos los pedidos registrados.
        Task<Respuesta<IEnumerable<Pedido>>> ListarAsync();

        // Permite modificar la información de un pedido existente.
        Task<Respuesta<Pedido>> ModificarAsync(Pedido datos);

        // Permite eliminar un pedido existente.
        Task<Respuesta<bool>> EliminarAsync(Pedido datos);

        // Permite buscar pedidos utilizando los datos recibidos.
        Task<Respuesta<IEnumerable<Pedido>>> BuscarAsync(Pedido datos);

        // Permite obtener un pedido específico por su identificador.
        Task<Respuesta<Pedido>> ObtenerAsync(Pedido datos);
    }
}
