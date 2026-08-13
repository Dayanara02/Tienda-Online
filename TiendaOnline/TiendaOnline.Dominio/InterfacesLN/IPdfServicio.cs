// Importa las entidades Pedido y Pago.
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.Dominio.InterfacesLN;

// Define la función para generar el comprobante PDF.
public interface IPdfServicio
{
    // Genera el PDF con los datos del pedido y del pago.
    byte[] GenerarComprobante(
        Pedido pedido,
        Pago pago
    );
}