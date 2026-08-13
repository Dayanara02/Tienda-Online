using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Esta interfaz define las operaciones de proformas.
    public interface IProformaLN
    {
        // Permite registrar una nueva proforma.
        Task<Respuesta<Proforma>> InsertarAsync(Proforma datos);

        // Permite obtener todas las proformas registradas.
        Task<Respuesta<IEnumerable<Proforma>>> ListarAsync();

        // Permite modificar la información de una proforma existente.
        Task<Respuesta<Proforma>> ModificarAsync(Proforma datos);

        // Permite eliminar una proforma existente.
        Task<Respuesta<bool>> EliminarAsync(Proforma datos);

        // Permite buscar proformas utilizando los datos recibidos.
        Task<Respuesta<IEnumerable<Proforma>>> BuscarAsync(Proforma datos);

        // Permite obtener una proforma específica por su identificador.
        Task<Respuesta<Proforma>> ObtenerAsync(Proforma datos);
    }
}
