// Permite utilizar funcionalidades básicas de C#.
using System;

// Permite trabajar con colecciones genéricas.
using System.Collections.Generic;

// Define el espacio de nombres de las entidades.
namespace TiendaOnline.Dominio.Entidades;

// Representa una notificación enviada a un usuario.
public partial class Notificacion
{
    // Identificador único de la notificación.
    public int IdNotificacion { get; set; }

    // Identificador del usuario que recibe la notificación.
    public int IdUsuario { get; set; }

    // Título que se muestra en la notificación.
    public string Titulo { get; set; } = null!;

    // Mensaje que contiene la información de la notificación.
    public string Mensaje { get; set; } = null!;

    // Tipo de notificación, si se desea clasificar.
    public string? Tipo { get; set; }

    // Fecha y hora en que se creó la notificación.
    public DateTime FechaCreacion { get; set; }

    // Indica si el usuario ya leyó la notificación.
    public bool Leida { get; set; }

    // Indica si la notificación se encuentra activa.
    public bool Estado { get; set; }

    // Relación con el usuario que recibe la notificación.
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}