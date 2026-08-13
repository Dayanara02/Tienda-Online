// Importa las funcionalidades necesarias para crear controladores y respuestas HTTP.
using Microsoft.AspNetCore.Mvc;

// Importa Entity Framework Core para realizar consultas a la base de datos.
using Microsoft.EntityFrameworkCore;

// Importa el contexto de la base de datos de la tienda.
using TiendaOnline.AccesoDatos.Context;

// Importa las herramientas para controlar el acceso de los usuarios.
using Microsoft.AspNetCore.Authorization;

// Importa las entidades utilizadas por el controlador.
using TiendaOnline.Dominio.Entidades;

// Define el espacio de nombres donde se encuentra el controlador.
namespace TiendaOnline.API.Controllers;

// Permite el acceso al controlador únicamente a usuarios autenticados.
[Authorize]

// Indica que esta clase funciona como un controlador de API.
[ApiController]

// Define la ruta principal para acceder al controlador.
[Route("api/[controller]")]
public class DetallePedidosController : ControllerBase
{
    // Guarda el contexto utilizado para acceder a la base de datos.
    private readonly TiendaOnlineContext _context;

    // Constructor que recibe el contexto mediante inyección de dependencias.
    public DetallePedidosController(TiendaOnlineContext context)
    {
        // Asigna el contexto recibido a la variable privada.
        _context = context;
    }

    // GET: api/DetallePedidos
    // Obtiene todos los detalles de pedidos registrados.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DetallePedido>>>
        GetDetallePedidos()
    {
        // Consulta los detalles sin realizar seguimiento de cambios.
        return await _context.DetallePedidos
            .AsNoTracking()

            // Ejecuta la consulta y obtiene todos los registros.
            .ToListAsync();
    }

    // GET: api/DetallePedidos/5
    // Obtiene un detalle de pedido específico mediante su ID.
    [HttpGet("{id}")]
    public async Task<ActionResult<DetallePedido>>
        GetDetallePedido(int id)
    {
        // Busca el detalle utilizando su identificador.
        var detalle = await _context.DetallePedidos.FindAsync(id);

        // Verifica si el detalle no existe.
        if (detalle == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Devuelve el detalle encontrado.
        return detalle;
    }

    // POST: api/DetallePedidos
    // Registra un nuevo detalle de pedido.
    [HttpPost]
    public async Task<ActionResult<DetallePedido>>
        PostDetallePedido(DetallePedido detalle)
    {
        // Establece el ID en cero para que la base de datos lo genere.
        detalle.IdDetallePedido = 0;

        // Agrega el detalle al contexto de la base de datos.
        _context.DetallePedidos.Add(detalle);

        // Guarda el nuevo registro en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta indicando que el detalle fue creado.
        return CreatedAtAction(
            // Indica la acción utilizada para consultar el detalle.
            nameof(GetDetallePedido),

            // Envía el ID del detalle creado.
            new { id = detalle.IdDetallePedido },

            // Devuelve los datos del detalle registrado.
            detalle
        );
    }

    // PUT: api/DetallePedidos/5
    // Actualiza un detalle de pedido existente.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDetallePedido(
        int id,
        DetallePedido detalle)
    {
        // Busca el detalle existente mediante su identificador.
        var detalleActual =
            await _context.DetallePedidos.FindAsync(id);

        // Verifica si el detalle no existe.
        if (detalleActual == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Actualiza el pedido asociado al detalle.
        detalleActual.IdPedido = detalle.IdPedido;

        // Actualiza el producto asociado al detalle.
        detalleActual.IdProducto = detalle.IdProducto;

        // Actualiza la cantidad de productos.
        detalleActual.Cantidad = detalle.Cantidad;

        // Actualiza el precio unitario del producto.
        detalleActual.PrecioUnitario = detalle.PrecioUnitario;

        // Actualiza el descuento aplicado.
        detalleActual.Descuento = detalle.Descuento;

        // Actualiza el impuesto correspondiente.
        detalleActual.Impuesto = detalle.Impuesto;

        // Actualiza el subtotal del detalle.
        detalleActual.Subtotal = detalle.Subtotal;

        // Guarda los cambios realizados en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se actualizó correctamente.
        return NoContent();
    }

    // DELETE: api/DetallePedidos/5
    // Elimina un detalle de pedido existente.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDetallePedido(int id)
    {
        // Busca el detalle mediante su identificador.
        var detalle = await _context.DetallePedidos.FindAsync(id);

        // Verifica si el detalle no existe.
        if (detalle == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Marca el detalle para ser eliminado.
        _context.DetallePedidos.Remove(detalle);

        // Guarda la eliminación en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se eliminó correctamente.
        return NoContent();
    }
}