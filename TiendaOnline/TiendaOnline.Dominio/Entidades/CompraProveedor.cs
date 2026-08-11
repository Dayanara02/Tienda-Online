using System; // Permite utilizar tipos como DateTime y decimal.
using System.Collections.Generic; // Permite trabajar con colecciones genéricas.


namespace TiendaOnline.Dominio.Entidades; 

public partial class CompraProveedor // Representa una compra realizada a un proveedor.
{
    public int IdCompraProveedor { get; set; } // Identificador único de la compra al proveedor.

    public int IdProveedor { get; set; } // Guarda el identificador del proveedor de la compra.

    public int IdUsuario { get; set; } // Guarda el identificador del usuario que realizó la compra.

    public DateTime FechaCompra { get; set; } // Guarda la fecha en que se realizó la compra.

    public decimal Subtotal { get; set; } // Guarda el monto de la compra antes de aplicar impuestos.

    public decimal Impuesto { get; set; } // Guarda el monto correspondiente al impuesto.

    public decimal Total { get; set; } // Guarda el monto total de la compra.

    public string Estado { get; set; } = null!; // Indica el estado actual de la compra.

    public virtual ICollection<DetalleCompraProveedor> DetalleCompraProveedors { get; set; } = new List<DetalleCompraProveedor>(); // Contiene los detalles de los productos incluidos en la compra.

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!; // Permite acceder a la información del proveedor relacionado.

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!; // Permite acceder a la información del usuario relacionado.
}
