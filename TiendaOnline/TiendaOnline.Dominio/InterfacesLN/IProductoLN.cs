using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Esta interfaz define las operaciones de lógica de negocio
    // que se pueden realizar con los productos.
    public interface IProductoLN
    {
        // Permite registrar un nuevo producto.
        Task<Respuesta<Producto>> InsertarAsync(Producto datos);

        // Permite obtener todos los productos registrados.
        Task<Respuesta<IEnumerable<Producto>>> ListarAsync();

        // Permite modificar la información de un producto existente.
        Task<Respuesta<Producto>> ModificarAsync(Producto datos);

        // Permite eliminar un producto existente.
        Task<Respuesta<bool>> EliminarAsync(Producto datos);

        // Permite buscar productos utilizando los datos recibidos.
        Task<Respuesta<IEnumerable<Producto>>> BuscarAsync(Producto datos);

        // Permite obtener un producto específico por su identificador.
        Task<Respuesta<Producto>> ObtenerAsync(Producto datos);
    }
}
