using System; // Permite utilizar tipos como decimal.
using System.Collections.Generic; // Permite trabajar con colecciones genéricas.

namespace TiendaOnline.Dominio.Entidades; 

public partial class DetalleCompraProveedor // Representa el detalle de una compra realizada a un proveedor.
{
    public int IdDetalleCompra { get; set; } // Identificador único del detalle de la compra.

    public int IdCompraProveedor { get; set; } // Guarda el identificador de la compra al proveedor.

    public int IdProducto { get; set; } // Guarda el identificador del producto comprado.

    public int Cantidad { get; set; } // Indica la cantidad de unidades compradas.

    public decimal PrecioUnitario { get; set; } // Guarda el precio de una unidad del producto.

    public decimal Subtotal { get; set; } // Guarda el monto total de este detalle antes de impuestos.

    public virtual CompraProveedor IdCompraProveedorNavigation { get; set; } = null!; // Permite acceder a la compra relacionada.

    public virtual Producto IdProductoNavigation { get; set; } = null!; // Permite acceder al producto relacionado.
}

