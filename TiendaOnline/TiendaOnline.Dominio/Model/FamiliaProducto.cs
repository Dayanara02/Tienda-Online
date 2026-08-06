using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Model;

public partial class FamiliaProducto
{
    public int IdFamilia { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<Categorium> Categoria { get; set; } = new List<Categorium>();

    public virtual ICollection<Descuento> IdDescuentos { get; set; } = new List<Descuento>();
}
