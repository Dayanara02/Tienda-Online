// Permite utilizar funcionalidades básicas de C#.
using System;

// Permite trabajar con colecciones genéricas.
using System.Collections.Generic;

// Define el espacio de nombres donde se encuentra la entidad tipada.
namespace TiendaOnline.Dominio.EntidadesTipadas;

// Representa la información de los productos más vendidos.
// Esta clase corresponde a una vista de la base de datos.
public partial class VwProductosMasVendido
{
    // Identificador único del producto.
    public int IdProducto { get; set; }

    // Nombre del producto.
    public string Producto { get; set; } = null!;

    // Cantidad total de unidades vendidas del producto.
    public int? CantidadVendida { get; set; }

    // Monto total obtenido por las ventas del producto.
    public decimal? TotalVentas { get; set; }
}