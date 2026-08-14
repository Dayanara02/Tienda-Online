using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Esta interfaz define las operaciones de inventario.
    public interface IInventarioLN
    {
        // Permite registrar un nuevo inventario.
        Task<Respuesta<Inventario>> InsertarAsync(Inventario datos);

        // Permite obtener todos los inventarios registrados.
        Task<Respuesta<IEnumerable<Inventario>>> ListarAsync();

        // Permite modificar la información de un inventario existente.
        Task<Respuesta<Inventario>> ModificarAsync(Inventario datos);

        // Permite eliminar un inventario existente.
        Task<Respuesta<bool>> EliminarAsync(Inventario datos);

        // Permite buscar inventarios utilizando los datos recibidos.
        Task<Respuesta<IEnumerable<Inventario>>> BuscarAsync(Inventario datos);

        // Permite obtener un inventario específico por su identificador.
        Task<Respuesta<Inventario>> ObtenerAsync(Inventario datos);
    }
}
