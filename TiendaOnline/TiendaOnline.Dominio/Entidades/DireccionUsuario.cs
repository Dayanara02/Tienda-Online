using System; // Permite utilizar tipos como bool y string.
using System.Collections.Generic; // Permite trabajar con colecciones como ICollection y List.

namespace TiendaOnline.Dominio.Entidades; 

public partial class DireccionUsuario // Representa una dirección registrada para un usuario.
{
    public int IdDireccion { get; set; } // Identificador único de la dirección.

    public int IdUsuario { get; set; } // Guarda el identificador del usuario dueño de la dirección.

    public string Provincia { get; set; } = null!; // Guarda la provincia donde se encuentra la dirección.

    public string Canton { get; set; } = null!; // Guarda el cantón correspondiente a la dirección.

    public string Distrito { get; set; } = null!; // Guarda el distrito donde se encuentra la dirección.

    public string DireccionExacta { get; set; } = null!; // Guarda los detalles específicos de la ubicación.

    public string? CodigoPostal { get; set; } // Guarda el código postal, si está disponible.

    public bool Principal { get; set; } // Indica si esta es la dirección principal del usuario.

    public bool Estado { get; set; } // Indica si la dirección está activa o inactiva.

    public virtual ICollection<Envio> Envios { get; set; } = new List<Envio>(); // Contiene los envíos asociados con esta dirección.

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!; // Permite acceder al usuario relacionado con la dirección.

    public virtual ICollection<Proforma> Proformas { get; set; } = new List<Proforma>(); // Contiene las proformas relacionadas con esta dirección.
}
