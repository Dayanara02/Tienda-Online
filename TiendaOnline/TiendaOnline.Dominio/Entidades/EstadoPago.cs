using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Entidades;

public partial class EstadoPago
{
    public int IdEstadoPago { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
