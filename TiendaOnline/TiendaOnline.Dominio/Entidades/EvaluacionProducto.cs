using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Entidades;

public partial class EvaluacionProducto
{
    public int IdEvaluacion { get; set; }

    public int IdProducto { get; set; }

    public int IdUsuario { get; set; }

    public int Calificacion { get; set; }

    public string? Comentario { get; set; }

    public DateTime FechaEvaluacion { get; set; }

    public bool Estado { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
