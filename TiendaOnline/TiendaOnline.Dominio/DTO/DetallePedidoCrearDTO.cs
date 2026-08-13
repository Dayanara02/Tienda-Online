namespace TiendaOnline.Dominio.DTO;

// DTO utilizado para recibir los datos necesarios para crear un detalle de pedido.
public class DetallePedidoCrearDto
{
    // Identificador del producto que se agregará al pedido.
    public int IdProducto { get; set; }

    // Cantidad de unidades del producto que se desea agregar.
    public int Cantidad { get; set; }
}