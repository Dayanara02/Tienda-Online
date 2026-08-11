using System; // Permite utilizar tipos como DateTime.
using System.Collections.Generic; // Permite trabajar con colecciones genéricas.

namespace TiendaOnline.Dominio.Entidades; 

public partial class Carrito // Representa el carrito de compras de un usuario.
{
    public int IdCarrito { get; set; } // Identificador único del carrito.

    public int IdUsuario { get; set; } // Guarda el identificador del usuario dueño del carrito.

    public DateTime FechaCreacion { get; set; } // Guarda la fecha en que se creó el carrito.

    public string Estado { get; set; } = null!; // Indica el estado actual del carrito.

    public virtual ICollection<DetalleCarrito> DetalleCarritos { get; set; } = new List<DetalleCarrito>(); // Guarda los detalles o productos que pertenecen al carrito.

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!; // Permite acceder al usuario relacionado con este carrito.
}
