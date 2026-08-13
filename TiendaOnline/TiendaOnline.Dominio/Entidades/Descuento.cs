using System; // Permite utilizar tipos del sistema, como DateOnly y decimal.
using System.Collections.Generic; // Permite trabajar con colecciones genéricas.

namespace TiendaOnline.Dominio.Entidades; 

public partial class Descuento // Representa un descuento que puede aplicarse a productos o categorías.
{
    public int IdDescuento { get; set; } // Identificador único del descuento.

    public string Nombre { get; set; } = null!; // Guarda el nombre del descuento.

    public string? Descripcion { get; set; } // Guarda una descripción del descuento, si existe.

    public decimal Porcentaje { get; set; } // Indica el porcentaje de descuento que se aplicará.

    public DateOnly FechaInicio { get; set; } // Guarda la fecha en que comienza el descuento.

    public DateOnly FechaFin { get; set; } // Guarda la fecha en que termina el descuento.

    public bool Estado { get; set; } // Indica si el descuento está activo o inactivo.

    public virtual ICollection<Categorium> IdCategoria { get; set; } = new List<Categorium>(); // Contiene las categorías relacionadas con el descuento.

    public virtual ICollection<FamiliaProducto> IdFamilia { get; set; } = new List<FamiliaProducto>(); // Contiene las familias de productos relacionadas con el descuento.

    public virtual ICollection<Producto> IdProductos { get; set; } = new List<Producto>(); // Contiene los productos relacionados con el descuento.
}

