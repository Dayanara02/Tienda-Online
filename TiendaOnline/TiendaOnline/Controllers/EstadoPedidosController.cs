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
public class EstadoPedidosController : ControllerBase
{
    // Guarda el contexto utilizado para acceder a la base de datos.
    private readonly TiendaOnlineContext _context;

    // Constructor que recibe el contexto mediante inyección de dependencias.
    public EstadoPedidosController(TiendaOnlineContext context)
    {
        // Asigna el contexto recibido a la variable privada.
        _context = context;
    }

    // GET: api/EstadoPedidos
    // Obtiene todos los estados de pedidos registrados.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EstadoPedido>>>
        GetEstadoPedidos()
    {
        // Consulta los estados sin realizar seguimiento de cambios.
        return await _context.EstadoPedidos
            .AsNoTracking()

            // Ejecuta la consulta y obtiene todos los registros.
            .ToListAsync();
    }

    // GET: api/EstadoPedidos/5
    // Obtiene un estado de pedido específico mediante su ID.
    [HttpGet("{id}")]
    public async Task<ActionResult<EstadoPedido>>
        GetEstadoPedido(int id)
    {
        // Busca el estado de pedido utilizando su identificador.
        var estadoPedido =
            await _context.EstadoPedidos.FindAsync(id);

        // Verifica si el estado de pedido no existe.
        if (estadoPedido == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Devuelve el estado de pedido encontrado.
        return estadoPedido;
    }

    // POST: api/EstadoPedidos
    // Registra un nuevo estado de pedido.
    [HttpPost]
    public async Task<ActionResult<EstadoPedido>>
        PostEstadoPedido(EstadoPedido estadoPedido)
    {
        // Establece el ID en cero para que la base de datos lo genere.
        estadoPedido.IdEstadoPedido = 0;

        // Verifica si ya existe un estado con el mismo nombre.
        var existe = await _context.EstadoPedidos
            .AnyAsync(e => e.Nombre == estadoPedido.Nombre);

        // Si ya existe, evita crear un registro duplicado.
        if (existe)
        {
            // Devuelve un error indicando que el nombre ya está registrado.
            return Conflict(
                "Ya existe un estado de pedido con ese nombre."
            );
        }

        // Agrega el nuevo estado al contexto.
        _context.EstadoPedidos.Add(estadoPedido);

        // Guarda el nuevo estado en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta indicando que el estado fue creado.
        return CreatedAtAction(
            // Indica la acción utilizada para consultar el estado.
            nameof(GetEstadoPedido),

            // Envía el ID del estado creado.
            new { id = estadoPedido.IdEstadoPedido },

            // Devuelve los datos del estado registrado.
            estadoPedido
        );
    }

    // PUT: api/EstadoPedidos/5
    // Actualiza un estado de pedido existente.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEstadoPedido(
        int id,
        EstadoPedido estadoPedido)
    {
        // Busca el estado existente mediante su identificador.
        var estadoActual =
            await _context.EstadoPedidos.FindAsync(id);

        // Verifica si el estado no existe.
        if (estadoActual == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Comprueba si otro estado utiliza el mismo nombre.
        var nombreExiste = await _context.EstadoPedidos
            .AnyAsync(e =>
                e.Nombre == estadoPedido.Nombre &&
                e.IdEstadoPedido != id);

        // Si el nombre ya existe, evita crear un duplicado.
        if (nombreExiste)
        {
            // Devuelve un error indicando que el nombre ya está registrado.
            return Conflict(
                "Ya existe otro estado de pedido con ese nombre."
            );
        }

        // Actualiza el nombre del estado.
        estadoActual.Nombre = estadoPedido.Nombre;

        // Actualiza la descripción del estado.
        estadoActual.Descripcion = estadoPedido.Descripcion;

        // Actualiza si el estado está activo o inactivo.
        estadoActual.Estado = estadoPedido.Estado;

        // Guarda los cambios realizados en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se actualizó correctamente.
        return NoContent();
    }

    // DELETE: api/EstadoPedidos/5
    // Elimina un estado de pedido existente.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEstadoPedido(int id)
    {
        // Busca el estado de pedido mediante su identificador.
        var estadoPedido =
            await _context.EstadoPedidos.FindAsync(id);

        // Verifica si el estado no existe.
        if (estadoPedido == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Comprueba si existen pedidos relacionados con este estado.
        var tienePedidos = await _context.Pedidos
            .AnyAsync(p => p.IdEstadoPedido == id);

        // Evita eliminar el estado si tiene pedidos asociados.
        if (tienePedidos)
        {
            // Devuelve un error indicando que existen registros relacionados.
            return Conflict(
                "No se puede eliminar porque existen pedidos relacionados."
            );
        }

        // Marca el estado de pedido para ser eliminado.
        _context.EstadoPedidos.Remove(estadoPedido);

        // Guarda la eliminación en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se eliminó correctamente.
        return NoContent();
    }
}