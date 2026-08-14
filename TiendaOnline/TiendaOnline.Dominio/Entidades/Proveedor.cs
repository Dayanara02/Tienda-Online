// Permite utilizar funcionalidades básicas de C#.
using System;

// Permite trabajar con colecciones genéricas.
using System.Collections.Generic;

// Importa las entidades del proyecto.
using TiendaOnline.Dominio.Entidades;

// Define el espacio de nombres de las entidades.
namespace TiendaOnline.Dominio.Entidades;

// Representa un proveedor de productos de la tienda.
public partial class Proveedor
{
    // Identificador único del proveedor.
    public int IdProveedor { get; set; }

    // Nombre del proveedor.
    public string Nombre { get; set; } = null!;

    // Número de identificación del proveedor.
    public string Identificacion { get; set; } = null!;

    // Correo electrónico del proveedor.
    public string? Correo { get; set; }

    // Número de teléfono del proveedor.
    public string? Telefono { get; set; }

    // Dirección física del proveedor.
    public string? Direccion { get; set; }

    // Indica si el proveedor se encuentra activo.
    public bool Estado { get; set; }

    // Compras realizadas a este proveedor.
    public virtual ICollection<CompraProveedor> CompraProveedors { get; set; } = new List<CompraProveedor>();

    // Productos relacionados con este proveedor.
    public virtual ICollection<ProductoProveedor> ProductoProveedors { get; set; } = new List<ProductoProveedor>();
}