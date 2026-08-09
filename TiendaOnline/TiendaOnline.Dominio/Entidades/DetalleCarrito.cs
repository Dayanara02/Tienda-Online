using System; // Permite utilizar tipos básicos del sistema, como decimal.
using System.Collections.Generic; // Permite trabajar con colecciones genéricas.

namespace TiendaOnline.Dominio.Entidades; 

public partial class DetalleCarrito // Representa el detalle de un producto dentro de un carrito.
{
    public int IdDetalleCarrito { get; set; } // Identificador único del detalle del carrito.

    public int IdCarrito { get; set; } // Guarda el identificador del carrito al que pertenece el detalle.

    public int IdProducto { get; set; } // Guarda el identificador del producto agregado al carrito.

    public int Cantidad { get; set; } // Indica cuántas unidades del producto se agregaron.

    public decimal PrecioUnitario { get; set; } // Guarda el precio de una unidad del producto.

    public virtual Carrito IdCarritoNavigation { get; set; } = null!; // Permite acceder al carrito relacionado con este detalle.

    public virtual Producto IdProductoNavigation { get; set; } = null!; // Permite acceder a la información del producto relacionado.
}
