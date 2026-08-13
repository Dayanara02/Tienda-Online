// Permite utilizar funcionalidades básicas de C#.
using System;

// Permite trabajar con colecciones genéricas.
using System.Collections.Generic;

// Define el espacio de nombres de las entidades.
namespace TiendaOnline.Dominio.Entidades;

// Representa un impuesto utilizado en los productos.
public partial class Impuesto
{
    // Identificador único del impuesto.
    public int IdImpuesto { get; set; }

    // Nombre del impuesto.
    public string Nombre { get; set; } = null!;

    // Porcentaje que se aplica como impuesto.
    public decimal Porcentaje { get; set; }

    // Indica si el impuesto está activo.
    public bool Estado { get; set; }

    // Relación con los productos que utilizan este impuesto.
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}