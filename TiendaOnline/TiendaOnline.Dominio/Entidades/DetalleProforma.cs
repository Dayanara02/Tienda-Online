using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Entidades;

public partial class DetalleProforma
{
    public int IdDetalleProforma { get; set; }

    public int IdProforma { get; set; }

    public int IdProducto { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Descuento { get; set; }

    public decimal Impuesto { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Proforma IdProformaNavigation { get; set; } = null!;
}
