using System; // Permite utilizar tipos como decimal.
using System.Collections.Generic; // Permite trabajar con colecciones genéricas.

namespace TiendaOnline.Dominio.Entidades; 

public partial class DetallePedido // Representa el detalle de un producto dentro de un pedido.
{
    public int IdDetallePedido { get; set; } // Identificador único del detalle del pedido.

    public int IdPedido { get; set; } // Guarda el identificador del pedido al que pertenece el detalle.

    public int IdProducto { get; set; } // Guarda el identificador del producto incluido en el pedido.

    public int Cantidad { get; set; } // Indica la cantidad de unidades del producto.

    public decimal PrecioUnitario { get; set; } // Guarda el precio de una unidad del producto.

    public decimal Descuento { get; set; } // Guarda el descuento aplicado al producto.

    public decimal Impuesto { get; set; } // Guarda el impuesto correspondiente al producto.

    public decimal Subtotal { get; set; } // Guarda el monto del detalle después de aplicar los valores correspondientes.

    public virtual Pedido IdPedidoNavigation { get; set; } = null!; // Permite acceder al pedido relacionado con este detalle.

    public virtual Producto IdProductoNavigation { get; set; } = null!; // Permite acceder a la información del producto relacionado.
}
