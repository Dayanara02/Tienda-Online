
namespace TiendaOnline.Dominio.DTO;

// Representa un producto comprado al proveedor.
public class DetalleCompraProveedorDto
{
    // Producto comprado.
    public int IdProducto { get; set; }

    // Cantidad comprada.
    public int Cantidad { get; set; }

    // Precio pagado por unidad.
    public decimal PrecioUnitario { get; set; }
}