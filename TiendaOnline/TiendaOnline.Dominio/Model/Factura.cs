using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Model;

public partial class Factura
{
    public int IdFactura { get; set; }

    public int IdPedido { get; set; }

    public string NumeroFactura { get; set; } = null!;

    public DateTime FechaEmision { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Impuesto { get; set; }

    public decimal Descuento { get; set; }

    public decimal Total { get; set; }

    public string? UrlPdf { get; set; }

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;
}
