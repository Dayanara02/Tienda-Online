using System; // Permite utilizar tipos básicos del sistema.
using System.Collections.Generic; // Permite trabajar con colecciones genéricas.

namespace TiendaOnline.Dominio.Entidades; 

public partial class Categorium // Representa una categoría de productos dentro de la tienda.
{
    public int IdCategoria { get; set; } // Identificador único de la categoría.

    public int IdFamilia { get; set; } // Guarda el identificador de la familia a la que pertenece la categoría.

    public string Nombre { get; set; } = null!; // Guarda el nombre de la categoría.

    public string? Descripcion { get; set; } // Guarda una descripción de la categoría, si existe.

    public bool Estado { get; set; } // Indica si la categoría está activa o inactiva.

    public virtual FamiliaProducto IdFamiliaNavigation { get; set; } = null!; // Permite acceder a la familia de productos relacionada.

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>(); // Contiene los productos que pertenecen a esta categoría.

    public virtual ICollection<Descuento> IdDescuentos { get; set; } = new List<Descuento>(); // Contiene los descuentos relacionados con esta categoría.
}
