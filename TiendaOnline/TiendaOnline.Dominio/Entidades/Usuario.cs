// Permite utilizar funcionalidades básicas de C#.
using System;

// Permite trabajar con colecciones genéricas.
using System.Collections.Generic;

// Importa las entidades del proyecto.
using TiendaOnline.Dominio.Entidades;

// Define el espacio de nombres de las entidades.
namespace TiendaOnline.Dominio.Entidades;

// Representa un usuario registrado en el sistema.
public partial class Usuario
{
    // Identificador único del usuario.
    public int IdUsuario { get; set; }

    // Identificador del rol asignado al usuario.
    public int IdRol { get; set; }

    // Nombre del usuario.
    public string Nombre { get; set; } = null!;

    // Apellido del usuario.
    public string Apellido { get; set; } = null!;

    // Correo electrónico utilizado por el usuario.
    public string Correo { get; set; } = null!;

    // Contraseña utilizada para iniciar sesión.
    public string Contrasena { get; set; } = null!;

    // Número de teléfono del usuario.
    public string? Telefono { get; set; }

    // Fecha en que se registró el usuario.
    public DateTime FechaRegistro { get; set; }

    // Indica si el usuario se encuentra activo.
    public bool Estado { get; set; }

    // Registros de acciones realizadas por el usuario.
    public virtual ICollection<BitacoraSistema> BitacoraSistemas { get; set; } = new List<BitacoraSistema>();

    // Carritos de compra relacionados con el usuario.
    public virtual ICollection<Carrito> Carritos { get; set; } = new List<Carrito>();

    // Compras realizadas por el usuario a proveedores.
    public virtual ICollection<CompraProveedor> CompraProveedors { get; set; } = new List<CompraProveedor>();

    // Direcciones registradas por el usuario.
    public virtual ICollection<DireccionUsuario> DireccionUsuarios { get; set; } = new List<DireccionUsuario>();

    // Evaluaciones realizadas por el usuario a productos.
    public virtual ICollection<EvaluacionProducto> EvaluacionProductos { get; set; } = new List<EvaluacionProducto>();

    // Historial de accesos realizados por el usuario.
    public virtual ICollection<HistorialAcceso> HistorialAccesos { get; set; } = new List<HistorialAcceso>();

    // Relación con el rol asignado al usuario.
    public virtual Rol IdRolNavigation { get; set; } = null!;

    // Lista de deseos del usuario, si tiene una registrada.
    public virtual ListaDeseo? ListaDeseo { get; set; }

    // Movimientos de inventario realizados por el usuario.
    public virtual ICollection<MovimientoInventario> MovimientoInventarios { get; set; } = new List<MovimientoInventario>();

    // Notificaciones recibidas por el usuario.
    public virtual ICollection<Notificacion> Notificacions { get; set; } = new List<Notificacion>();

    // Pedidos realizados por el usuario.
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();

    // Proformas creadas por el usuario.
    public virtual ICollection<Proforma> Proformas { get; set; } = new List<Proforma>();
}