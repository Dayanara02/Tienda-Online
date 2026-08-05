namespace TiendaOnline.Dominio.DTO;

public class PedidoCrearDto
{
    public string? DireccionEntrega { get; set; }

    public List<DetallePedidoCrearDto> Detalles { get; set; } = new();
}