using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Model;

public partial class HistorialAcceso
{
    public int IdHistorialAcceso { get; set; }

    public int IdUsuario { get; set; }

    public DateTime FechaAcceso { get; set; }

    public string? DireccionIp { get; set; }

    public bool Exitoso { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
