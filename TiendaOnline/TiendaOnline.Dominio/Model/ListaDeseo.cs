using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Model;

public partial class ListaDeseo
{
    public int IdListaDeseos { get; set; }

    public int IdUsuario { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual ICollection<DetalleListaDeseo> DetalleListaDeseos { get; set; } = new List<DetalleListaDeseo>();

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
