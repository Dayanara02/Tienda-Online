using System;
using System.Collections.Generic;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.Dominio.Entidades;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int IdRol { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public string? Telefono { get; set; }

    public DateTime FechaRegistro { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<BitacoraSistema> BitacoraSistemas { get; set; } = new List<BitacoraSistema>();

    public virtual ICollection<Carrito> Carritos { get; set; } = new List<Carrito>();

    public virtual ICollection<CompraProveedor> CompraProveedors { get; set; } = new List<CompraProveedor>();

    public virtual ICollection<DireccionUsuario> DireccionUsuarios { get; set; } = new List<DireccionUsuario>();

    public virtual ICollection<EvaluacionProducto> EvaluacionProductos { get; set; } = new List<EvaluacionProducto>();

    public virtual ICollection<HistorialAcceso> HistorialAccesos { get; set; } = new List<HistorialAcceso>();

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ListaDeseo? ListaDeseo { get; set; }

    public virtual ICollection<MovimientoInventario> MovimientoInventarios { get; set; } = new List<MovimientoInventario>();

    public virtual ICollection<Notificacion> Notificacions { get; set; } = new List<Notificacion>();

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    public virtual ICollection<Proforma> Proformas { get; set; } = new List<Proforma>();
}
