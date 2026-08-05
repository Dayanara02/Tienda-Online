using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Model;

public partial class VwCatalogoProducto
{
    public int IdProducto { get; set; }

    public string Codigo { get; set; } = null!;

    public string Producto { get; set; } = null!;

    public string? Imagen { get; set; }

    public string Familia { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public decimal Precio { get; set; }

    public decimal Costo { get; set; }

    public decimal Impuesto { get; set; }

    public int Stock { get; set; }

    public bool Estado { get; set; }
}
