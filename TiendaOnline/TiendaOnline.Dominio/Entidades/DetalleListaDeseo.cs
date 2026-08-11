using System; // Permite utilizar tipos como DateTime.
using System.Collections.Generic; // Permite trabajar con colecciones genéricas.

namespace TiendaOnline.Dominio.Entidades; 

public partial class DetalleListaDeseo // Representa un producto guardado dentro de una lista de deseos.
{
    public int IdListaDeseos { get; set; } // Identificador de la lista de deseos a la que pertenece.

    public int IdProducto { get; set; } // Guarda el identificador del producto agregado a la lista.

    public DateTime FechaAgregado { get; set; } // Guarda la fecha y hora en que se agregó el producto.

    public virtual ListaDeseo IdListaDeseosNavigation { get; set; } = null!; // Permite acceder a la lista de deseos relacionada.

    public virtual Producto IdProductoNavigation { get; set; } = null!; // Permite acceder a la información del producto relacionado.
}
