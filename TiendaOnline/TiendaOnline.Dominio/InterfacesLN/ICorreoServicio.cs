namespace TiendaOnline.Dominio.InterfacesLN;

// Define las funciones para enviar correos.
public interface ICorreoServicio
{
    // Envía el comprobante PDF al cliente.
    Task EnviarComprobanteAsync(
        string destinatario,
        string nombreCliente,
        int idPedido,
        byte[] archivoPdf
    );
}