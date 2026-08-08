using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Entidades;

public partial class Impuesto
{
    public int IdImpuesto { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Porcentaje { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
