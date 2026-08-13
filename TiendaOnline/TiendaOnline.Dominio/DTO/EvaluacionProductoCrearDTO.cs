
namespace TiendaOnline.Dominio.DTO;

// Guarda los datos para evaluar un producto.
public class EvaluacionProductoCrearDto
{
    // Producto que será evaluado.
    public int IdProducto { get; set; }

    // Calificación del producto.
    public int Calificacion { get; set; }

    // Comentario opcional.
    public string? Comentario { get; set; }
}