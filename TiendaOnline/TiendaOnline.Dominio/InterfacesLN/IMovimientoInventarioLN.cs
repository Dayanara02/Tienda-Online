using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Esta interfaz define las operaciones de movimiento de inventario.
    public interface IMovimientoInventarioLN
    {
        // Permite registrar un nuevo movimiento de inventario.
        Task<Respuesta<MovimientoInventario>> InsertarAsync(MovimientoInventario datos);

        // Permite obtener todos los movimientos de inventario registrados.
        Task<Respuesta<IEnumerable<MovimientoInventario>>> ListarAsync();

        // Permite modificar la información de un movimiento de inventario existente.
        Task<Respuesta<MovimientoInventario>> ModificarAsync(MovimientoInventario datos);

        // Permite eliminar un movimiento de inventario existente.
        Task<Respuesta<bool>> EliminarAsync(MovimientoInventario datos);

        // Permite buscar movimientos de inventario utilizando los datos recibidos.
        Task<Respuesta<IEnumerable<MovimientoInventario>>> BuscarAsync(MovimientoInventario datos);

        // Permite obtener un movimiento de inventario específico por su identificador.
        Task<Respuesta<MovimientoInventario>> ObtenerAsync(MovimientoInventario datos);
    }
}
