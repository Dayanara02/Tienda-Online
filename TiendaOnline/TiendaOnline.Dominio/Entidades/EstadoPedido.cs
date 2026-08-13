using System; // Permite utilizar tipos básicos del sistema, como bool.
using System.Collections.Generic; // Permite trabajar con colecciones como ICollection y List.

namespace TiendaOnline.Dominio.Entidades; 

public partial class EstadoPedido // Representa los diferentes estados que puede tener un pedido.
{
    public int IdEstadoPedido { get; set; } // Identificador único del estado del pedido.

    public string Nombre { get; set; } = null!; // Guarda el nombre del estado del pedido.

    public string? Descripcion { get; set; } // Guarda una descripción del estado, si existe.

    public bool Estado { get; set; } // Indica si este estado de pedido está activo o inactivo.

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>(); // Contiene los pedidos que utilizan este estado.
}

