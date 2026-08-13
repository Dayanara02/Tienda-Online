using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Entidades;

public partial class Pago
{
    public int IdPago { get; set; }

    public int IdPedido { get; set; }

    public string MetodoPago { get; set; } = null!;

    public string? Referencia { get; set; }

    public decimal Monto { get; set; }

    public DateTime? FechaPago { get; set; }

    public string Estado { get; set; } = null!;

    public int? IdMetodoPago { get; set; }

    public int? IdEstadoPago { get; set; }

    public virtual EstadoPago? IdEstadoPagoNavigation { get; set; }

    public virtual MetodoPago? IdMetodoPagoNavigation { get; set; }

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;
}
