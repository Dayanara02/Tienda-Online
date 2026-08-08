using System;
using System.Collections.Generic;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.Dominio.Entidades;

public partial class CompraProveedor
{
    public int IdCompraProveedor { get; set; }

    public int IdProveedor { get; set; }

    public int IdUsuario { get; set; }

    public DateTime FechaCompra { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Impuesto { get; set; }

    public decimal Total { get; set; }

    public string Estado { get; set; } = null!;

    public virtual ICollection<DetalleCompraProveedor> DetalleCompraProveedors { get; set; } = new List<DetalleCompraProveedor>();

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
