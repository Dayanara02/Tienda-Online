using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Entidades;

public partial class Descuento
{
    public int IdDescuento { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Porcentaje { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<Categorium> IdCategoria { get; set; } = new List<Categorium>();

    public virtual ICollection<FamiliaProducto> IdFamilia { get; set; } = new List<FamiliaProducto>();

    public virtual ICollection<Producto> IdProductos { get; set; } = new List<Producto>();
}
