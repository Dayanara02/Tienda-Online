using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Model;

public partial class ProductoProveedor
{
    public int IdProducto { get; set; }

    public int IdProveedor { get; set; }

    public decimal PrecioCompra { get; set; }

    public string? CodigoProveedor { get; set; }

    public bool Estado { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;
}
