using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Model;

public partial class Inventario
{
    public int IdInventario { get; set; }

    public int IdProducto { get; set; }

    public int CantidadDisponible { get; set; }

    public DateTime FechaActualizacion { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual ICollection<MovimientoInventario> MovimientoInventarios { get; set; } = new List<MovimientoInventario>();
}
