// Permite trabajar con fechas y horas.
using System;

// Permite utilizar colecciones genéricas.
using System.Collections.Generic;

// Define el espacio de nombres donde se encuentra la entidad.
namespace TiendaOnline.Dominio.Entidades;

// Representa el historial de accesos realizados por los usuarios.
public partial class HistorialAcceso
{
    // Identificador único del registro de acceso.
    public int IdHistorialAcceso { get; set; }

    // Identificador del usuario que realizó el acceso.
    public int IdUsuario { get; set; }

    // Fecha y hora en que se realizó el acceso.
    public DateTime FechaAcceso { get; set; }

    // Dirección IP desde donde se realizó el acceso.
    public string? DireccionIp { get; set; }

    // Indica si el intento de acceso fue exitoso.
    public bool Exitoso { get; set; }

    // Relación con el usuario que realizó el acceso.
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}