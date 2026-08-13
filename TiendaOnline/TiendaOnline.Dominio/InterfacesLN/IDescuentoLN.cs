using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesLN
{
    // Esta interfaz define las operaciones de Descuentos.
    public interface IDescuentoLN
    {
        // Permite registrar un nuevo descuento.
        Task<Respuesta<Descuento>> InsertarAsync(Descuento datos);

        // Permite listar todos los descuentos.
        Task<Respuesta<IEnumerable<Descuento>>> ListarAsync();

        // Permite modificar un descuento existente
        Task<Respuesta<Descuento>> ModificarAsync(Descuento datos);

        // Permite eliminar un descuento existente.
        Task<Respuesta<bool>> EliminarAsync(Descuento datos);

        // Permite buscar un descuento utilizando los datos recibidos.
        Task<Respuesta<IEnumerable<Descuento>>> BuscarAsync(Descuento datos);

        // Permite obtener un descuento específico por su identificador.
        Task<Respuesta<Descuento>> ObtenerAsync(Descuento datos);
    }
}
