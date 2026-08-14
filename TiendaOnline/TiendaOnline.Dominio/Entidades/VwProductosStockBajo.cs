// Permite utilizar funcionalidades básicas de C#.
using System;


namespace TiendaOnline.Dominio.Entidades;

// Representa los productos que tienen un nivel de inventario bajo.
// Esta clase corresponde a una vista de la base de datos.
public partial class VwProductosStockBajo
{
    // Identificador único del producto.
    public int IdProducto { get; set; }

    // Nombre del producto.
    public string Nombre { get; set; } = null!;

    // Cantidad de unidades disponibles actualmente.
    public int CantidadDisponible { get; set; }

    // Cantidad mínima de unidades que debe mantenerse en inventario.
    public int StockMinimo { get; set; }
}