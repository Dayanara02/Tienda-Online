namespace TiendaOnline.Dominio.DTO;

// DTO que contiene la información de un pedido creado.
public class PedidoCreadoDto
{
    // Identificador único del pedido.
    public int IdPedido { get; set; }

    // Fecha y hora en que se realizó el pedido.
    public DateTime FechaPedido { get; set; }

    // Monto total de los productos antes de impuestos y descuentos.
    public decimal Subtotal { get; set; }

    // Monto correspondiente al impuesto aplicado al pedido.
    public decimal Impuesto { get; set; }

    // Monto de descuento aplicado al pedido.
    public decimal Descuento { get; set; }

    // Monto total que debe pagar el cliente.
    public decimal Total { get; set; }

    // Estado actual del pedido.
    public string Estado { get; set; } = string.Empty;

    // Mensaje informativo relacionado con la creación del pedido.
    public string Mensaje { get; set; } = string.Empty;
}