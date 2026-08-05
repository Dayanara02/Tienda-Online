using TiendaOnline.Dominio.DTO;

namespace TiendaOnline.LogicaNegocio.Interfaces;

public interface IPedidoServicio
{
    Task<PedidoCreadoDto> CrearPedidoAsync(
        int idUsuario,
        PedidoCrearDto pedido);
}