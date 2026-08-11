using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Esta interfaz define las operaciones de lógica de negocio
    // que se pueden realizar con las categorías.
    public interface ICategoriaLN
    {
        // Permite registrar una nueva categoría.
        Task<Respuesta<Categorium>> InsertarAsync(Categorium datos);

        // Permite obtener todas las categorías registradas.
        Task<Respuesta<IEnumerable<Categorium>>> ListarAsync();

        // Permite modificar una categoría existente.
        Task<Respuesta<Categorium>> ModificarAsync(Categorium datos);

        // Permite eliminar una categoría existente.
        Task<Respuesta<bool>> EliminarAsync(Categorium datos);

        // Permite buscar categorías por el nombre recibido.
        Task<Respuesta<IEnumerable<Categorium>>> BuscarAsync(Categorium datos);

        // Permite obtener una categoría específica por su identificador.
        Task<Respuesta<Categorium>> ObtenerAsync(Categorium datos);
    }
}
