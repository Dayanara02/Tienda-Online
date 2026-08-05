using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Model;

public partial class DireccionUsuario
{
    public int IdDireccion { get; set; }

    public int IdUsuario { get; set; }

    public string Provincia { get; set; } = null!;

    public string Canton { get; set; } = null!;

    public string Distrito { get; set; } = null!;

    public string DireccionExacta { get; set; } = null!;

    public string? CodigoPostal { get; set; }

    public bool Principal { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<Envio> Envios { get; set; } = new List<Envio>();

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Proforma> Proformas { get; set; } = new List<Proforma>();
}
