// Permite utilizar tipos básicos de C#.
using System;

// Permite utilizar colecciones.
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Entidades;

// Representa el inventario de un producto.
public partial class Inventario
{
    // Identificador único del inventario.
    public int IdInventario { get; set; }

    // Identificador del producto relacionado.
    public int IdProducto { get; set; }

    // Cantidad disponible del producto.
    public int CantidadDisponible { get; set; }

    // Fecha de la última actualización.
    public DateTime FechaActualizacion { get; set; }

    // Relación con el producto.
    public virtual Producto IdProductoNavigation { get; set; } = null!;

    // Movimientos realizados sobre este inventario.
    public virtual ICollection<MovimientoInventario> MovimientoInventarios { get; set; }
        = new List<MovimientoInventario>();
}