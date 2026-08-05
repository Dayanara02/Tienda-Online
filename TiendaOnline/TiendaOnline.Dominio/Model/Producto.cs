using System;
using System.Collections.Generic;
using TiendaOnline.Dominio.Model;

namespace TiendaOnline.Dominio.Model;

public partial class Producto
{
    public int IdProducto { get; set; }

    public int IdCategoria { get; set; }

    public int IdImpuesto { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public string Codigo { get; set; } = null!;

    public decimal Precio { get; set; }

    public decimal Costo { get; set; }

    public string? Imagen { get; set; }

    public int StockMinimo { get; set; }

    public bool Estado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual ICollection<DetalleCarrito> DetalleCarritos { get; set; } = new List<DetalleCarrito>();

    public virtual ICollection<DetalleCompraProveedor> DetalleCompraProveedors { get; set; } = new List<DetalleCompraProveedor>();

    public virtual ICollection<DetalleListaDeseo> DetalleListaDeseos { get; set; } = new List<DetalleListaDeseo>();

    public virtual ICollection<DetallePedido> DetallePedidos { get; set; } = new List<DetallePedido>();

    public virtual ICollection<DetalleProforma> DetalleProformas { get; set; } = new List<DetalleProforma>();

    public virtual ICollection<EvaluacionProducto> EvaluacionProductos { get; set; } = new List<EvaluacionProducto>();

    public virtual Categorium? IdCategoriaNavigation { get; set; }

    public virtual Impuesto? IdImpuestoNavigation { get; set; }

    public virtual Inventario? Inventario { get; set; }

    public virtual ICollection<ProductoProveedor> ProductoProveedors { get; set; } = new List<ProductoProveedor>();

    public virtual ICollection<Descuento> IdDescuentos { get; set; } = new List<Descuento>();
}