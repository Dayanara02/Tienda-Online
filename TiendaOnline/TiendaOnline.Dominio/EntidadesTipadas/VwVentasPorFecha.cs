// Permite utilizar funcionalidades básicas de C#.
using System;

// Permite trabajar con colecciones genéricas.
using System.Collections.Generic;

// Define el espacio de nombres donde se encuentra la entidad tipada.
namespace TiendaOnline.Dominio.EntidadesTipadas;

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