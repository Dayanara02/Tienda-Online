// Define el espacio de nombres.
namespace TiendaOnline.Dominio.EntidadesTipadas;

// Representa una categoría simplificada.
public class TCategoria
{
    // Identificador de la categoría.
    public int IdCategoria { get; set; }

    // Identificador de la familia.
    public int IdFamilia { get; set; }

    // Nombre de la categoría.
    public string Nombre { get; set; } = null!;

    // Descripción de la categoría.
    public string? Descripcion { get; set; }

    // Indica si está activa.
    public bool Estado { get; set; }
}