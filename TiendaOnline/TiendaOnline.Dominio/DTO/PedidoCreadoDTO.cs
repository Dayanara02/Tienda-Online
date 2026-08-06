namespace TiendaOnline.Dominio.DTO;

public class PedidoCreadoDto
{
    public int IdPedido { get; set; }

    public DateTime FechaPedido { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Impuesto { get; set; }

    public decimal Descuento { get; set; }

    public decimal Total { get; set; }

    public string Estado { get; set; } = string.Empty;

    public string Mensaje { get; set; } = string.Empty;
}