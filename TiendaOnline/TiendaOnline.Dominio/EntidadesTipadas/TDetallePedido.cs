// Define el espacio de nombres de las entidades tipadas.
namespace TiendaOnline.Dominio.EntidadesTipadas;

// Representa los datos tipados de un detalle de pedido.
public class TDetallePedido
{
    // Identificador del detalle.
    public int IdDetallePedido { get; set; }

    // Identificador del pedido.
    public int IdPedido { get; set; }

    // Identificador del producto.
    public int IdProducto { get; set; }

    // Cantidad del producto.
    public int Cantidad { get; set; }

    // Precio unitario del producto.
    public decimal PrecioUnitario { get; set; }

    // Descuento aplicado.
    public decimal Descuento { get; set; }

    // Impuesto aplicado.
    public decimal Impuesto { get; set; }

    // Subtotal del detalle.
    public decimal Subtotal { get; set; }
}