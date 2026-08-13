using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Esta interfaz define las operaciones de Evualuación de Productos
    public interface IEvaluacionProductoLN
    {
        // Permite registrar un nueva evaluación de producto.
        Task<Respuesta<EvaluacionProducto>> InsertarAsync(EvaluacionProducto datos);

        // Permite listar todas las evaluaciones de productos registradas.
        Task<Respuesta<IEnumerable<EvaluacionProducto>>> ListarAsync();

        // Permite modificar la información de una evaluación de producto existente.
        Task<Respuesta<EvaluacionProducto>> ModificarAsync(EvaluacionProducto datos);

        // Permite eliminar una evaluación de producto existente.
        Task<Respuesta<bool>> EliminarAsync(EvaluacionProducto datos);

        // Permite buscar evaluaciones de productos utilizando los datos recibidos.
        Task<Respuesta<IEnumerable<EvaluacionProducto>>> BuscarAsync(EvaluacionProducto datos);

        // Permite obtener una evaluación de producto específica por su identificador.
        Task<Respuesta<EvaluacionProducto>> ObtenerAsync(EvaluacionProducto datos);
    }
}
