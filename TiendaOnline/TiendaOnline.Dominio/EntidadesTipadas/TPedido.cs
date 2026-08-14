// Permite utilizar DateTime.
using System;

// Define el espacio de nombres.
namespace TiendaOnline.Dominio.EntidadesTipadas;

// Representa la información principal de un pedido.
public class TPedido
{
    // Identificador del pedido.
    public int IdPedido { get; set; }

    // Identificador del usuario.
    public int IdUsuario { get; set; }

    // Fecha en que se realizó el pedido.
    public DateTime FechaPedido { get; set; }

    // Estado actual del pedido.
    public string Estado { get; set; } = null!;

    // Monto antes de impuesto y descuento.
    public decimal Subtotal { get; set; }

    // Impuesto aplicado.
    public decimal Impuesto { get; set; }

    // Descuento aplicado.
    public decimal Descuento { get; set; }

    // Total del pedido.
    public decimal Total { get; set; }

    // Dirección de entrega.
    public string? DireccionEntrega { get; set; }

    // Identificador del estado del pedido.
    public int? IdEstadoPedido { get; set; }
}