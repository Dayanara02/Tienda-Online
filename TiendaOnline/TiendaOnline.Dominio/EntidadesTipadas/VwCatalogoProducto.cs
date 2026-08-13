
using System;

// Permite trabajar con colecciones genéricas.
using System.Collections.Generic;

// Define el espacio de nombres donde se encuentra la entidad tipada.
namespace TiendaOnline.Dominio.EntidadesTipadas;

// Representa la información del catálogo de productos.
// Esta clase corresponde a una vista de la base de datos.
public partial class VwCatalogoProducto
{
    // Identificador único del producto.
    public int IdProducto { get; set; }

    // Código utilizado para identificar el producto.
    public string Codigo { get; set; } = null!;

    // Nombre del producto mostrado en el catálogo.
    public string Producto { get; set; } = null!;

    // Imagen asociada al producto.
    public string? Imagen { get; set; }

    // Familia a la que pertenece el producto.
    public string Familia { get; set; } = null!;

    // Categoría del producto.
    public string Categoria { get; set; } = null!;

    // Precio de venta del producto.
    public decimal Precio { get; set; }

    // Costo de adquisición del producto.
    public decimal Costo { get; set; }

    // Impuesto aplicado al producto.
    public decimal Impuesto { get; set; }

    // Cantidad disponible del producto en inventario.
    public int Stock { get; set; }

    // Indica si el producto se encuentra activo.
    public bool Estado { get; set; }
}