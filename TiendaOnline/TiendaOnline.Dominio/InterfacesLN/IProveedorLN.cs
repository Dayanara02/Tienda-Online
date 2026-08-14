using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Esta interfaz define las operaciones de proveedores.
    public interface IProveedorLN
    {
        // Permite registrar un nuevo proveedor.
        Task<Respuesta<Proveedor>> InsertarAsync(Proveedor datos);

        // Permite obtener todos los proveedores registrados.
        Task<Respuesta<IEnumerable<Proveedor>>> ListarAsync();

        // Permite modificar la información de un proveedor existente.
        Task<Respuesta<Proveedor>> ModificarAsync(Proveedor datos);

        // Permite eliminar un proveedor existente.
        Task<Respuesta<bool>> EliminarAsync(Proveedor datos);

        // Permite buscar proveedores utilizando los datos recibidos.
        Task<Respuesta<IEnumerable<Proveedor>>> BuscarAsync(Proveedor datos);

        // Permite obtener un proveedor específico por su identificador.
        Task<Respuesta<Proveedor>> ObtenerAsync(Proveedor datos);
    }
}
