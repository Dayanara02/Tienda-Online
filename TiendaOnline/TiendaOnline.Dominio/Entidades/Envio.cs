using System; // Permite utilizar tipos como DateTime.
using System.Collections.Generic; // Permite trabajar con colecciones genéricas.

namespace TiendaOnline.Dominio.Entidades; 

public partial class Envio // Representa el envío de un pedido realizado por un usuario.
{
    public int IdEnvio { get; set; } // Identificador único del envío.

    public int IdPedido { get; set; } // Guarda el identificador del pedido que se está enviando.

    public int IdDireccion { get; set; } // Guarda el identificador de la dirección donde se realizará la entrega.

    public string? EmpresaEnvio { get; set; } // Guarda el nombre de la empresa encargada del envío.

    public string? NumeroSeguimiento { get; set; } // Guarda el número utilizado para rastrear el envío.

    public DateTime? FechaEnvio { get; set; } // Guarda la fecha y hora en que se realizó el envío.

    public DateTime? FechaEntrega { get; set; } // Guarda la fecha y hora en que se entregó el pedido.

    public string Estado { get; set; } = null!; // Indica el estado actual del envío.

    public virtual DireccionUsuario IdDireccionNavigation { get; set; } = null!; // Permite acceder a la dirección relacionada con el envío.

    public virtual Pedido IdPedidoNavigation { get; set; } = null!; // Permite acceder al pedido relacionado con el envío.
}

