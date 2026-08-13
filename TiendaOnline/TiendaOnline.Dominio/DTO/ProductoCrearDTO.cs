
namespace TiendaOnline.Dominio.DTO;

// Guarda los datos para crear un producto.
public class ProductoCrearDto
{
    // Categoría del producto.
    public int IdCategoria { get; set; }

    // Impuesto aplicado al producto.
    public int IdImpuesto { get; set; }

    // Nombre del producto.
    public string Nombre { get; set; } =
        string.Empty;

    // Descripción del producto.
    public string? Descripcion { get; set; }

    // Código único del producto.
    public string Codigo { get; set; } =
        string.Empty;

    // Precio de venta.
    public decimal Precio { get; set; }

    // Costo del producto.
    public decimal Costo { get; set; }

    // Ruta o URL de la imagen.
    public string? Imagen { get; set; }

    // Cantidad mínima recomendada.
    public int StockMinimo { get; set; }

    // Indica si está activo.
    public bool Estado { get; set; } = true;
}