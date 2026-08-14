// Permite utilizar tipos básicos de C#.
using System;

// Permite utilizar colecciones.
using System.Collections.Generic;

namespace TiendaOnline.Dominio.Entidades;

// Representa un movimiento realizado en el inventario.
public partial class MovimientoInventario
{
    // Identificador único del movimiento.
    public int IdMovimiento { get; set; }

    // Identificador del inventario relacionado.
    public int IdInventario { get; set; }

    // Identificador del usuario que realizó el movimiento.
    public int IdUsuario { get; set; }

    // Indica el tipo de movimiento realizado.
    public string TipoMovimiento { get; set; } = null!;

    // Cantidad de productos involucrados.
    public int Cantidad { get; set; }

    // Describe la razón del movimiento.
    public string? Motivo { get; set; }

    // Fecha en que se realizó el movimiento.
    public DateTime FechaMovimiento { get; set; }

    // Relación con el inventario.
    public virtual Inventario IdInventarioNavigation { get; set; } = null!;

    // Relación con el usuario.
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}