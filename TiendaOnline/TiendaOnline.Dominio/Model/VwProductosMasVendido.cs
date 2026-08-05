using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Model;

public partial class VwProductosMasVendido
{
    public int IdProducto { get; set; }

    public string Producto { get; set; } = null!;

    public int? CantidadVendida { get; set; }

    public decimal? TotalVentas { get; set; }
}
