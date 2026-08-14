
namespace TiendaOnline.Dominio.DTO;

// Guarda los datos necesarios para crear un envío.
public class CrearEnvioDto
{
    // Pedido que será enviado.
    public int IdPedido { get; set; }

    // Dirección de entrega.
    public int IdDireccion { get; set; }

    // Empresa encargada del envío.
    public string? EmpresaEnvio { get; set; }

    // Número utilizado para seguimiento.
    public string? NumeroSeguimiento { get; set; }
}