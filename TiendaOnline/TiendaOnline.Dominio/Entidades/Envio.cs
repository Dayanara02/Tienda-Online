using System;
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Entidades;

public partial class Envio
{
    public int IdEnvio { get; set; }

    public int IdPedido { get; set; }

    public int IdDireccion { get; set; }

    public string? EmpresaEnvio { get; set; }

    public string? NumeroSeguimiento { get; set; }

    public DateTime? FechaEnvio { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public string Estado { get; set; } = null!;

    public virtual DireccionUsuario IdDireccionNavigation { get; set; } = null!;

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;
}
