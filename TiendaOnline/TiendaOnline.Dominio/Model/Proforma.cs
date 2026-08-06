using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Model;

public partial class Proforma
{
    public int IdProforma { get; set; }

    public int IdUsuario { get; set; }

    public int? IdDireccion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateOnly? FechaVencimiento { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Impuesto { get; set; }

    public decimal Descuento { get; set; }

    public decimal Total { get; set; }

    public string Estado { get; set; } = null!;

    public string? UrlPdf { get; set; }

    public virtual ICollection<DetalleProforma> DetalleProformas { get; set; } = new List<DetalleProforma>();

    public virtual DireccionUsuario? IdDireccionNavigation { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
