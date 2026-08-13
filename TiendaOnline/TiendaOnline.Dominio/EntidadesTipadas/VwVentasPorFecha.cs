using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.EntidadesTipadas;

public partial class VwVentasPorFecha
{
    public DateOnly? Fecha { get; set; }

    public int? CantidadPedidos { get; set; }

    public decimal? Ingresos { get; set; }
}
