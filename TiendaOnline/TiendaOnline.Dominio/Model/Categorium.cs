using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Model;

public partial class Categorium
{
    public int IdCategoria { get; set; }

    public int IdFamilia { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Estado { get; set; }

    public virtual FamiliaProducto IdFamiliaNavigation { get; set; } = null!;

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();

    public virtual ICollection<Descuento> IdDescuentos { get; set; } = new List<Descuento>();
}
