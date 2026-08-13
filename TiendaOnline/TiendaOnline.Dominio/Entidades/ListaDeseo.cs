// Permite utilizar tipos básicos de C#.
using System;

// Permite utilizar colecciones.
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Entidades;

// Representa la lista de deseos de un usuario.
public partial class ListaDeseo
{
    // Identificador único de la lista de deseos.
    public int IdListaDeseos { get; set; }

    // Identificador del usuario relacionado.
    public int IdUsuario { get; set; }

    // Fecha en que se creó la lista.
    public DateTime FechaCreacion { get; set; }

    // Productos guardados en la lista de deseos.
    public virtual ICollection<DetalleListaDeseo> DetalleListaDeseos { get; set; }
        = new List<DetalleListaDeseo>();

    // Relación con el usuario.
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}