// Permite utilizar funcionalidades básicas de C#.
using System;

// Permite trabajar con colecciones genéricas.
using System.Collections.Generic;

// Define el espacio de nombres de las entidades.
namespace TiendaOnline.Dominio.Entidades;

// Representa un pago realizado para un pedido.
public partial class Pago
{
    // Identificador único del pago.
    public int IdPago { get; set; }

    // Identificador del pedido al que pertenece el pago.
    public int IdPedido { get; set; }

    // Método utilizado para realizar el pago.
    public string MetodoPago { get; set; } = null!;

    // Referencia o comprobante del pago, si existe.
    public string? Referencia { get; set; }

    // Monto correspondiente al pago realizado.
    public decimal Monto { get; set; }

    // Fecha y hora en que se realizó el pago.
    public DateTime? FechaPago { get; set; }

    // Estado actual del pago.
    public string Estado { get; set; } = null!;

    // Identificador opcional del método de pago.
    public int? IdMetodoPago { get; set; }

    // Identificador opcional del estado del pago.
    public int? IdEstadoPago { get; set; }

    // Relación con el estado del pago.
    public virtual EstadoPago? IdEstadoPagoNavigation { get; set; }

    // Relación con el método de pago.
    public virtual MetodoPago? IdMetodoPagoNavigation { get; set; }

    // Relación con el pedido al que pertenece el pago.
    public virtual Pedido IdPedidoNavigation { get; set; } = null!;
}