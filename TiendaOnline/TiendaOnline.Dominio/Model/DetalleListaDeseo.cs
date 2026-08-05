using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Model;

public partial class DetalleListaDeseo
{
    public int IdListaDeseos { get; set; }

    public int IdProducto { get; set; }

    public DateTime FechaAgregado { get; set; }

    public virtual ListaDeseo IdListaDeseosNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
