// Permite utilizar funcionalidades básicas de C#.
using System;

// Permite trabajar con colecciones genéricas.
using System.Collections.Generic;

// Define el espacio de nombres donde se encuentran las entidades.
namespace TiendaOnline.Dominio.Entidades;

// Representa una familia de productos dentro de la tienda.
public partial class FamiliaProducto
{
    // Identificador único de la familia.
    public int IdFamilia { get; set; }

    // Nombre de la familia de productos.
    public string Nombre { get; set; } = null!;

    // Descripción opcional de la familia.
    public string? Descripcion { get; set; }

    // Indica si la familia se encuentra activa.
    public bool Estado { get; set; }

    // Relación con las categorías pertenecientes a esta familia.
    public virtual ICollection<Categorium> Categoria { get; set; } = new List<Categorium>();

    // Relación con los descuentos asociados a esta familia.
    public virtual ICollection<Descuento> IdDescuentos { get; set; } = new List<Descuento>();
}