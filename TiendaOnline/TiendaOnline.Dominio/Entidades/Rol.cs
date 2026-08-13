// Permite utilizar funcionalidades básicas de C#.
using System;

// Permite trabajar con colecciones genéricas.
using System.Collections.Generic;

// Define el espacio de nombres de las entidades.
namespace TiendaOnline.Dominio.Entidades;

// Representa un rol que puede tener un usuario del sistema.
public partial class Rol
{
    // Identificador único del rol.
    public int IdRol { get; set; }

    // Nombre del rol.
    public string Nombre { get; set; } = null!;

    // Descripción opcional de las funciones del rol.
    public string? Descripcion { get; set; }

    // Indica si el rol se encuentra activo.
    public bool Estado { get; set; }

    // Usuarios que tienen asignado este rol.
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}