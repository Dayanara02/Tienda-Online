
namespace TiendaOnline.Dominio.DTO;

// Guarda los datos para modificar un producto.
public class ProductoModificarDto
{
    // Producto que se modificará.
    public int IdProducto { get; set; }

    // Categoría del producto.
    public int IdCategoria { get; set; }

    // Impuesto aplicado.
    public int IdImpuesto { get; set; }

    // Nombre del producto.
    public string Nombre { get; set; } =
        string.Empty;

    // Descripción del producto.
    public string? Descripcion { get; set; }

    // Código del producto.
    public string Codigo { get; set; } =
        string.Empty;

    // Precio de venta.
    public decimal Precio { get; set; }

    // Costo del producto.
    public decimal Costo { get; set; }

    // Imagen del producto.
    public string? Imagen { get; set; }

    // Stock mínimo permitido.
    public int StockMinimo { get; set; }

    // Indica si está activo.
    public bool Estado { get; set; }
}