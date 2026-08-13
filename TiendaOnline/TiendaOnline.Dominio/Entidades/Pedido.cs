
using System;
// Permite trabajar con colecciones genéricas.
using System.Collections.Generic;
namespace TiendaOnline.Dominio.Entidades;

public partial class Pedido
{
    // Identificador único del pedido.
    public int IdPedido { get; set; }

    // Guarda el usuario propietario del pedido.
    public int IdUsuario { get; set; }

    // Guarda la fecha en que se realizó el pedido.
    public DateTime FechaPedido { get; set; }

    // Guarda el estado actual del pedido.
    public string Estado { get; set; } = null!;

    // Guarda el monto antes de descuento e impuesto.
    public decimal Subtotal { get; set; }

    // Guarda el impuesto total aplicado.
    public decimal Impuesto { get; set; }

    // Guarda el descuento total aplicado.
    public decimal Descuento { get; set; }

    // Guarda el monto final del pedido.
    public decimal Total { get; set; }

    // Guarda la dirección elegida al confirmar el pedido.
    public string? DireccionEntrega { get; set; }

    // Guarda el identificador del estado relacionado.
    public int? IdEstadoPedido { get; set; }

    // Contiene los productos incluidos en el pedido.
    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } =
        new List<DetallePedido>();

    // Permite acceder al envío relacionado con el pedido.
    public virtual Envio? Envio { get; set; }

    // Permite acceder a la factura relacionada.
    public virtual Factura? Factura { get; set; }

    // Permite acceder al estado relacionado.
    public virtual EstadoPedido? IdEstadoPedidoNavigation { get; set; }

    // Permite acceder al usuario propietario del pedido.
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    // Contiene los pagos realizados para el pedido.
    public virtual ICollection<Pago> Pagos { get; set; } =
        new List<Pago>();
}