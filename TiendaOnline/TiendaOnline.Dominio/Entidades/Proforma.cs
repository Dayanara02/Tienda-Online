// Permite utilizar tipos básicos de C#.
using System;

// Permite utilizar colecciones.
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Entidades;

// Representa una proforma generada para un usuario.
public partial class Proforma
{
    // Identificador único de la proforma.
    public int IdProforma { get; set; }

    // Identificador del usuario relacionado.
    public int IdUsuario { get; set; }

    // Identificador de la dirección seleccionada.
    public int? IdDireccion { get; set; }

    // Fecha en que se creó la proforma.
    public DateTime FechaCreacion { get; set; }

    // Fecha hasta la que es válida la proforma.
    public DateOnly? FechaVencimiento { get; set; }

    // Subtotal antes de impuestos y descuentos.
    public decimal Subtotal { get; set; }

    // Monto correspondiente a impuestos.
    public decimal Impuesto { get; set; }

    // Monto de descuento aplicado.
    public decimal Descuento { get; set; }

    // Total final de la proforma.
    public decimal Total { get; set; }

    // Estado actual de la proforma.
    public string Estado { get; set; } = null!;

    // Ruta del archivo PDF generado.
    public string? UrlPdf { get; set; }

    // Detalles incluidos en la proforma.
    public virtual ICollection<DetalleProforma> DetalleProformas { get; set; }
        = new List<DetalleProforma>();

    // Relación con la dirección.
    public virtual DireccionUsuario? IdDireccionNavigation { get; set; }

    // Relación con el usuario.
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}