using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Esta interfaz define las operaciones de compras a proveedores.
    public interface ICompraProveedorLN
    {
        // Permite registrar una compra a proveedor.
        Task<Respuesta<CompraProveedor>> InsertarAsync(CompraProveedor datos);

        // Permite listar las compras realizadas.
        Task<Respuesta<IEnumerable<CompraProveedor>>> ListarAsync();

        // Permite modificar una compra existente.
        Task<Respuesta<CompraProveedor>> ModificarAsync(CompraProveedor datos);

        // Permite eliminar una compra registrada.
        Task<Respuesta<bool>> EliminarAsync(CompraProveedor datos);

        // Permite buscar compras.
        Task<Respuesta<IEnumerable<CompraProveedor>>> BuscarAsync(CompraProveedor datos);

        // Permite obtener una compra específica.
        Task<Respuesta<CompraProveedor>> ObtenerAsync(CompraProveedor datos);
    }
}
