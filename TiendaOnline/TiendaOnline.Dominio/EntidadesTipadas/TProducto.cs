// Permite utilizar DateTime.
using System;

// Define el espacio de nombres.
namespace TiendaOnline.Dominio.EntidadesTipadas;

// Representa la información principal de un producto.
public class TProducto
{
    // Identificador del producto.
    public int IdProducto { get; set; }

    // Identificador de la categoría.
    public int IdCategoria { get; set; }

    // Identificador del impuesto.
    public int IdImpuesto { get; set; }

    // Nombre del producto.
    public string Nombre { get; set; } = null!;

    // Descripción del producto.
    public string? Descripcion { get; set; }

    // Código del producto.
    public string Codigo { get; set; } = null!;

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

    // Fecha de registro.
    public DateTime FechaRegistro { get; set; }
}