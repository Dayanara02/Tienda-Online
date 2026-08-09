using System; // Permite utilizar tipos básicos del sistema, como bool.
using System.Collections.Generic; // Permite trabajar con colecciones como ICollection y List.

namespace TiendaOnline.Dominio.Entidades; 

public partial class EstadoPago // Representa los diferentes estados que puede tener un pago.
{
    public int IdEstadoPago { get; set; } // Identificador único del estado del pago.

    public string Nombre { get; set; } = null!; // Guarda el nombre del estado del pago.

    public string? Descripcion { get; set; } // Guarda una descripción del estado, si existe.

    public bool Estado { get; set; } // Indica si este estado de pago está activo o inactivo.

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>(); // Contiene los pagos que utilizan este estado.
}
