using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Model;

public partial class EstadoPedido
{
    public int IdEstadoPedido { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
