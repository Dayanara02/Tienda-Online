// Importa las clases necesarias para trabajar con fechas.
using System;

// Importa las colecciones genéricas de C#.
using System.Collections.Generic;

// Define el espacio de nombres donde se encuentra la entidad.
namespace TiendaOnline.Dominio.Entidades;

// Representa una evaluación realizada a un producto.
public partial class EvaluacionProducto
{
    // Identificador único de la evaluación.
    public int IdEvaluacion { get; set; }

    // Identificador del producto evaluado.
    public int IdProducto { get; set; }

    // Identificador del usuario que realizó la evaluación.
    public int IdUsuario { get; set; }

    // Calificación otorgada al producto.
    public int Calificacion { get; set; }

    // Comentario opcional escrito por el usuario.
    public string? Comentario { get; set; }

    // Fecha en que se realizó la evaluación.
    public DateTime FechaEvaluacion { get; set; }

    // Indica si la evaluación se encuentra activa.
    public bool Estado { get; set; }

    // Relación con el producto evaluado.
    public virtual Producto IdProductoNavigation { get; set; } = null!;

    // Relación con el usuario que realizó la evaluación.
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}