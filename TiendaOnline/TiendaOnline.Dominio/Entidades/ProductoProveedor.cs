// Permite utilizar funcionalidades básicas de C#.
using System;

// Permite trabajar con colecciones genéricas.
using System.Collections.Generic;

// Define el espacio de nombres de las entidades.
namespace TiendaOnline.Dominio.Entidades;

// Representa la relación entre un producto y un proveedor.
public partial class ProductoProveedor
{
    // Identificador del producto relacionado.
    public int IdProducto { get; set; }

    // Identificador del proveedor relacionado.
    public int IdProveedor { get; set; }

    // Precio de compra del producto al proveedor.
    public decimal PrecioCompra { get; set; }

    // Código que utiliza el proveedor para identificar el producto.
    public string? CodigoProveedor { get; set; }

    // Indica si la relación entre el producto y el proveedor está activa.
    public bool Estado { get; set; }

    // Relación con el producto.
    public virtual Producto IdProductoNavigation { get; set; } = null!;

    // Relación con el proveedor.
    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;
}