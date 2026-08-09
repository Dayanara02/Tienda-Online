using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Esta interfaz define las operaciones de lógica de negocio
    // que se pueden realizar con los pedidos.
    public interface IPedidoLN
    {
        // Registra un nuevo pedido en el sistema.
        Task<Respuesta<Pedido>> InsertarAsync(Pedido datos);

        // Obtiene todos los pedidos registrados.
        Task<Respuesta<IEnumerable<Pedido>>> ListarAsync();

        // Modifica los datos generales de un pedido existente.
        Task<Respuesta<Pedido>> ModificarAsync(Pedido datos);

        // Elimina un pedido utilizando su identificador.
        Task<Respuesta<bool>> EliminarAsync(Pedido datos);

        // Busca pedidos utilizando los datos recibidos como filtro.
        Task<Respuesta<IEnumerable<Pedido>>> BuscarAsync(Pedido datos);

        // Obtiene un pedido específico por su IdPedido.
        Task<Respuesta<Pedido>> ObtenerAsync(Pedido datos);
    }
}
