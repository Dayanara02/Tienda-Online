using TiendaOnline.Dominio.DTO;

namespace TiendaOnline.Dominio.InterfacesLN;

public interface IPedidoServicio
{
    Task<PedidoCreadoDto> CrearPedidoAsync(
        int idUsuario,
        PedidoCrearDto pedido);
}