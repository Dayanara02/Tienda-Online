using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Model;

public partial class DetalleCompraProveedor
{
    public int IdDetalleCompra { get; set; }

    public int IdCompraProveedor { get; set; }

    public int IdProducto { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public virtual CompraProveedor IdCompraProveedorNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
