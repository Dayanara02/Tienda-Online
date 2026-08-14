using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Esta interfaz define las operaciones del carrito.
    public interface ICarritoLN
    {
        // Permite registrar un carrito.
        Task<Respuesta<Carrito>> InsertarAsync(Carrito datos);

        // Permite listar los carritos.
        Task<Respuesta<IEnumerable<Carrito>>>
            ListarAsync();

        // Permite modificar un carrito.
        Task<Respuesta<Carrito>> ModificarAsync(Carrito datos);

        // Permite eliminar un carrito
        Task<Respuesta<bool>> EliminarAsync(Carrito datos);

        // Permite buscar carritos.
        Task<Respuesta<IEnumerable<Carrito>>> BuscarAsync(Carrito datos);

        // Permite obtener un carrito específico.
        Task<Respuesta<Carrito>> ObtenerAsync(Carrito datos);
    }
}
