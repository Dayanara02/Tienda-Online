using System; // Permite utilizar tipos como decimal.
using System.Collections.Generic; // Permite trabajar con colecciones genéricas.

namespace TiendaOnline.Dominio.Entidades; 

public partial class DetalleProforma // Representa el detalle de un producto dentro de una proforma.
{
    public int IdDetalleProforma { get; set; } // Identificador único del detalle de la proforma.

    public int IdProforma { get; set; } // Guarda el identificador de la proforma a la que pertenece el detalle.

    public int IdProducto { get; set; } // Guarda el identificador del producto incluido en la proforma.

    public int Cantidad { get; set; } // Indica la cantidad de unidades del producto.

    public decimal PrecioUnitario { get; set; } // Guarda el precio de una unidad del producto.

    public decimal Descuento { get; set; } // Guarda el descuento aplicado al producto.

    public decimal Impuesto { get; set; } // Guarda el impuesto correspondiente al producto.

    public decimal Subtotal { get; set; } // Guarda el monto total de este detalle.

    public virtual Producto IdProductoNavigation { get; set; } = null!; // Permite acceder a la información del producto relacionado.

    public virtual Proforma IdProformaNavigation { get; set; } = null!; // Permite acceder a la proforma relacionada.
}
