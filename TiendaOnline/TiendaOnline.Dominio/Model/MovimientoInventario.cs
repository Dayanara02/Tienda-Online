using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Model;

public partial class MovimientoInventario
{
    public int IdMovimiento { get; set; }

    public int IdInventario { get; set; }

    public int IdUsuario { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    public int Cantidad { get; set; }

    public string? Motivo { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public virtual Inventario IdInventarioNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
