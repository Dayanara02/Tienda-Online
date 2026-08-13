// Permite utilizar funcionalidades básicas de C#.
using System;

// Permite trabajar con colecciones genéricas.
using System.Collections.Generic;

// Define el espacio de nombres de las entidades.
namespace TiendaOnline.Dominio.Entidades;

// Representa un método de pago disponible en la tienda.
public partial class MetodoPago
{
    // Identificador único del método de pago.
    public int IdMetodoPago { get; set; }

    // Nombre del método de pago.
    public string Nombre { get; set; } = null!;

    // Descripción opcional del método de pago.
    public string? Descripcion { get; set; }

    // Indica si el método de pago está activo.
    public bool Estado { get; set; }

    // Relación con los pagos realizados mediante este método.
    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}