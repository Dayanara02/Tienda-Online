// Permite utilizar funcionalidades básicas de C#.
using System;

// Permite trabajar con colecciones genéricas.
using System.Collections.Generic;

// Define el espacio de nombres de las entidades.
namespace TiendaOnline.Dominio.Entidades;

// Representa un producto disponible en la tienda.
public partial class Producto
{
    // Identificador único del producto.
    public int IdProducto { get; set; }

    // Identificador de la categoría a la que pertenece.
    public int IdCategoria { get; set; }

    // Identificador del impuesto aplicado al producto.
    public int IdImpuesto { get; set; }

    // Nombre del producto.
    public string Nombre { get; set; } = null!;

    // Descripción opcional del producto.
    public string? Descripcion { get; set; }

    // Código utilizado para identificar el producto.
    public string Codigo { get; set; } = null!;

    // Precio de venta del producto.
    public decimal Precio { get; set; }

    // Costo de adquisición del producto.
    public decimal Costo { get; set; }

    // Ruta o dirección de la imagen del producto.
    public string? Imagen { get; set; }

    // Cantidad mínima de unidades que debe mantenerse en inventario.
    public int StockMinimo { get; set; }

    // Indica si el producto se encuentra activo.
    public bool Estado { get; set; }

    // Fecha en que se registró el producto.
    public DateTime FechaRegistro { get; set; }

    // Productos relacionados con los detalles de los carritos.
    public virtual ICollection<DetalleCarrito> DetalleCarritos { get; set; } = new List<DetalleCarrito>();

    // Productos relacionados con compras realizadas a proveedores.
    public virtual ICollection<DetalleCompraProveedor> DetalleCompraProveedors { get; set; } = new List<DetalleCompraProveedor>();

    // Productos incluidos en listas de deseos.
    public virtual ICollection<DetalleListaDeseo> DetalleListaDeseos { get; set; } = new List<DetalleListaDeseo>();

    // Productos incluidos en los detalles de los pedidos.
    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    // Productos incluidos en los detalles de las proformas.
    public virtual ICollection<DetalleProforma> DetalleProformas { get; set; } = new List<DetalleProforma>();

    // Evaluaciones realizadas sobre este producto.
    public virtual ICollection<EvaluacionProducto> EvaluacionProductos { get; set; } = new List<EvaluacionProducto>();

    // Relación con la categoría del producto.
    public virtual Categorium? IdCategoriaNavigation { get; set; }

    // Relación con el impuesto aplicado al producto.
    public virtual Impuesto? IdImpuestoNavigation { get; set; }

    // Relación con el registro de inventario del producto.
    public virtual Inventario? Inventario { get; set; }

    // Proveedores relacionados con el producto.
    public virtual ICollection<ProductoProveedor> ProductoProveedors { get; set; } = new List<ProductoProveedor>();

    // Descuentos asociados al producto.
    public virtual ICollection<Descuento> IdDescuentos { get; set; } = new List<Descuento>();
}