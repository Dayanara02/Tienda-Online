
namespace TiendaOnline.Dominio.DTO;

// Guarda los datos necesarios para pagar un pedido.
public class PagarPedidoDto
{
    // Identificador del pedido.
    public int IdPedido { get; set; }

    // Método de pago seleccionado.
    public int IdMetodoPago { get; set; }
}