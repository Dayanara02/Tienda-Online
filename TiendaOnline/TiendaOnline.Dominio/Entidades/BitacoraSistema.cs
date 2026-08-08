using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Entidades;

public partial class BitacoraSistema
{
    public int IdBitacora { get; set; }

    public int IdUsuario { get; set; }

    public string Accion { get; set; } = null!;

    public string? TablaAfectada { get; set; }

    public int? IdRegistro { get; set; }

    public string? Descripcion { get; set; }

    public DateTime Fecha { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
