// Permite utilizar tipos básicos del sistema.
using System;

// Define el espacio de nombres de las entidades.
namespace TiendaOnline.Dominio.Entidades;

// Representa un descuento disponible en la tienda.
public partial class Descuento
{
    // Identificador único del descuento.
    public int IdDescuento { get; set; }

    // Nombre del descuento.
    public string Nombre { get; set; } = null!;

    // Descripción del descuento.
    public string? Descripcion { get; set; }

    // Porcentaje que se aplicará.
    public decimal Porcentaje { get; set; }

    // Fecha en que inicia el descuento.
    public DateOnly FechaInicio { get; set; }

    // Fecha en que finaliza el descuento.
    public DateOnly FechaFin { get; set; }

    // Indica si el descuento está activo.
    public bool Estado { get; set; }

    // Cantidad mínima de productos requerida.
    public int CantidadMinima { get; set; }

    // Categorías relacionadas con el descuento.
    public virtual ICollection<Categorium> IdCategoria { get; set; }
        = new List<Categorium>();

    // Familias relacionadas con el descuento.
    public virtual ICollection<FamiliaProducto> IdFamilia { get; set; }
        = new List<FamiliaProducto>();

    // Productos relacionados con el descuento.
    public virtual ICollection<Producto> IdProductos { get; set; }
        = new List<Producto>();
}