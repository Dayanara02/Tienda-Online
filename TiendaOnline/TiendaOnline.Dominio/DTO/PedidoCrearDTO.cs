// Define el espacio de nombres donde se encuentran los DTO del proyecto.
namespace TiendaOnline.Dominio.DTO;

// Representa la información que recibe la API para crear un pedido.
public class PedidoCrearDto
{
    // Guarda la dirección donde se entregará el pedido.
    public string? DireccionEntrega { get; set; }

    // Guarda el identificador de la promoción seleccionada.
    public int? IdPromocion { get; set; }

    // Guarda el porcentaje de descuento seleccionado por el cliente.
    public decimal PorcentajeDescuento { get; set; }

    // Guarda los productos que forman parte del pedido.
    public List<DetallePedidoCrearDto> Detalles { get; set; } = new();
}