using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Model;

public partial class VwProductosStockBajo
{
    public int IdProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public int CantidadDisponible { get; set; }

    public int StockMinimo { get; set; }
}
