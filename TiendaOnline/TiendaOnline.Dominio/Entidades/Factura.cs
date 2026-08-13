// Permite trabajar con fechas y horas.
using System;

// Permite utilizar colecciones genéricas.
using System.Collections.Generic;

// Define el espacio de nombres de las entidades del sistema.
namespace TiendaOnline.Dominio.Entidades;

// Representa una factura generada para un pedido.
public partial class Factura
{
    // Identificador único de la factura.
    public int IdFactura { get; set; }

    // Identificador del pedido relacionado con la factura.
    public int IdPedido { get; set; }

    // Número único utilizado para identificar la factura.
    public string NumeroFactura { get; set; } = null!;

    // Fecha en que se emitió la factura.
    public DateTime FechaEmision { get; set; }

    // Monto correspondiente a los productos antes de impuestos y descuentos.
    public decimal Subtotal { get; set; }

    // Monto calculado por concepto de impuestos.
    public decimal Impuesto { get; set; }

    // Monto descontado del total de la compra.
    public decimal Descuento { get; set; }

    // Monto final que debe pagar el cliente.
    public decimal Total { get; set; }

    // Dirección donde se encuentra almacenado el archivo PDF de la factura.
    public string? UrlPdf { get; set; }

    // Relación con el pedido al que pertenece la factura.
    public virtual Pedido IdPedidoNavigation { get; set; } = null!;
}