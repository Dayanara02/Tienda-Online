using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Entidades;

public partial class Pedido
{
    public int IdPedido { get; set; }

    public int IdUsuario { get; set; }

    public DateTime FechaPedido { get; set; }

    public string Estado { get; set; } = null!;

    public decimal Subtotal { get; set; }

    public decimal Impuesto { get; set; }

    public decimal Descuento { get; set; }

    public decimal Total { get; set; }

    public string? DireccionEntrega { get; set; }

    public int? IdEstadoPedido { get; set; }

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    public virtual Envio? Envio { get; set; }

    public virtual Factura? Factura { get; set; }

    public virtual EstadoPedido? IdEstadoPedidoNavigation { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
