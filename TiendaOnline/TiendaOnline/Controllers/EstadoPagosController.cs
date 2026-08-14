// Importa las funcionalidades necesarias para crear controladores y respuestas HTTP.
using Microsoft.AspNetCore.Mvc;

// Importa Entity Framework Core para realizar consultas a la base de datos.
using Microsoft.EntityFrameworkCore;

// Importa el contexto de la base de datos de la tienda.
using TiendaOnline.AccesoDatos.Context;

// Importa las herramientas para controlar el acceso mediante roles.
using Microsoft.AspNetCore.Authorization;

// Importa las entidades utilizadas por el controlador.
using TiendaOnline.Dominio.Entidades;

// Define el espacio de nombres donde se encuentra el controlador.
namespace TiendaOnline.API.Controllers;

// Permite el acceso únicamente a usuarios con rol Administrador.
[Authorize(Roles = "Administrador")]

// Indica que esta clase funciona como un controlador de API.
[ApiController]

// Define la ruta principal para acceder al controlador.
[Route("api/[controller]")]
public class EstadoPagosController : ControllerBase
{
    // Guarda el contexto utilizado para acceder a la base de datos.
    private readonly TiendaOnlineContext _context;

    // Constructor que recibe el contexto mediante inyección de dependencias.
    public EstadoPagosController(TiendaOnlineContext context)
    {
        // Asigna el contexto recibido a la variable privada.
        _context = context;
    }

    // GET: api/EstadoPagos
    // Obtiene todos los estados de pago registrados.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EstadoPago>>>
        GetEstadoPagos()
    {
        // Consulta los estados sin realizar seguimiento de cambios.
        return await _context.EstadoPagos
            .AsNoTracking()

            // Ejecuta la consulta y obtiene todos los registros.
            .ToListAsync();
    }

    // GET: api/EstadoPagos/5
    // Obtiene un estado de pago específico mediante su ID.
    [HttpGet("{id}")]
    public async Task<ActionResult<EstadoPago>>
        GetEstadoPago(int id)
    {
        // Busca el estado de pago utilizando su identificador.
        var estadoPago =
            await _context.EstadoPagos.FindAsync(id);

        // Verifica si el estado de pago no existe.
        if (estadoPago == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Devuelve el estado de pago encontrado.
        return estadoPago;
    }

    // POST: api/EstadoPagos
    // Registra un nuevo estado de pago.
    [HttpPost]
    public async Task<ActionResult<EstadoPago>>
        PostEstadoPago(EstadoPago estadoPago)
    {
        // Establece el ID en cero para que la base de datos lo genere.
        estadoPago.IdEstadoPago = 0;

        // Verifica si ya existe un estado con el mismo nombre.
        var existe = await _context.EstadoPagos
            .AnyAsync(e => e.Nombre == estadoPago.Nombre);

        // Si ya existe, evita crear un registro duplicado.
        if (existe)
        {
            // Devuelve un error indicando que el nombre ya está registrado.
            return Conflict(
                "Ya existe un estado de pago con ese nombre."
            );
        }

        // Agrega el nuevo estado al contexto.
        _context.EstadoPagos.Add(estadoPago);

        // Guarda el nuevo estado en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta indicando que el estado fue creado.
        return CreatedAtAction(
            // Indica la acción utilizada para consultar el estado.
            nameof(GetEstadoPago),

            // Envía el ID del estado creado.
            new { id = estadoPago.IdEstadoPago },

            // Devuelve los datos del estado registrado.
            estadoPago
        );
    }

    // PUT: api/EstadoPagos/5
    // Actualiza un estado de pago existente.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEstadoPago(
        int id,
        EstadoPago estadoPago)
    {
        // Busca el estado existente mediante su identificador.
        var estadoActual =
            await _context.EstadoPagos.FindAsync(id);

        // Verifica si el estado no existe.
        if (estadoActual == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Comprueba si otro estado utiliza el mismo nombre.
        var nombreExiste = await _context.EstadoPagos
            .AnyAsync(e =>
                e.Nombre == estadoPago.Nombre &&
                e.IdEstadoPago != id);

        // Si el nombre ya existe, evita crear un duplicado.
        if (nombreExiste)
        {
            // Devuelve un error indicando que el nombre ya está registrado.
            return Conflict(
                "Ya existe otro estado de pago con ese nombre."
            );
        }

        // Actualiza el nombre del estado de pago.
        estadoActual.Nombre = estadoPago.Nombre;

        // Actualiza la descripción del estado.
        estadoActual.Descripcion = estadoPago.Descripcion;

        // Actualiza el estado activo o inactivo.
        estadoActual.Estado = estadoPago.Estado;

        // Guarda los cambios realizados en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se actualizó correctamente.
        return NoContent();
    }

    // DELETE: api/EstadoPagos/5
    // Elimina un estado de pago existente.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEstadoPago(int id)
    {
        // Busca el estado de pago mediante su identificador.
        var estadoPago =
            await _context.EstadoPagos.FindAsync(id);

        // Verifica si el estado no existe.
        if (estadoPago == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Comprueba si existen pagos relacionados con este estado.
        var tienePagos = await _context.Pagos
            .AnyAsync(p => p.IdEstadoPago == id);

        // Evita eliminar el estado si tiene pagos asociados.
        if (tienePagos)
        {
            // Devuelve un error indicando que existen registros relacionados.
            return Conflict(
                "No se puede eliminar porque existen pagos relacionados."
            );
        }

        // Marca el estado de pago para ser eliminado.
        _context.EstadoPagos.Remove(estadoPago);

        // Guarda la eliminación en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se eliminó correctamente.
        return NoContent();
    }
}