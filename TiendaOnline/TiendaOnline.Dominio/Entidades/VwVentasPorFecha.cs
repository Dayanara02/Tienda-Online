// Permite utilizar funcionalidades básicas de C#.
using System;

namespace TiendaOnline.Dominio.Entidades;

// Representa la información de las ventas agrupadas por fecha.
// Esta clase corresponde a una vista de la base de datos.
public partial class VwVentasPorFecha
{
    // Fecha en la que se realizaron las ventas.
    public DateOnly? Fecha { get; set; }

    // Cantidad de pedidos registrados en esa fecha.
    public int? CantidadPedidos { get; set; }

    // Ingresos totales obtenidos en esa fecha.
    public decimal? Ingresos { get; set; }
}