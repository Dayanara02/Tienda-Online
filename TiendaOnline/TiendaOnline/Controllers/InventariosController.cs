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

// Indica que esta clase funciona como un controlador de API.
[ApiController]

// Define la ruta principal para acceder al controlador.
[Route("api/[controller]")]
public class InventariosController : ControllerBase
{
    // Guarda el contexto utilizado para acceder a la base de datos.
    private readonly TiendaOnlineContext _context;

    // Constructor que recibe el contexto mediante inyección de dependencias.
    public InventariosController(TiendaOnlineContext context)
    {
        // Asigna el contexto recibido a la variable privada.
        _context = context;
    }

    // GET: api/Inventarios
    // Obtiene todos los registros de inventario.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Inventario>>> GetInventarios()
    {
        // Consulta los inventarios sin realizar seguimiento de cambios.
        return await _context.Inventarios
            .AsNoTracking()

            // Ejecuta la consulta y obtiene todos los registros.
            .ToListAsync();
    }

    // GET: api/Inventarios/5
    // Obtiene un registro de inventario específico mediante su ID.
    [HttpGet("{id}")]
    public async Task<ActionResult<Inventario>> GetInventario(int id)
    {
        // Busca el inventario utilizando su identificador.
        var inventario = await _context.Inventarios.FindAsync(id);

        // Verifica si el inventario no existe.
        if (inventario == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Devuelve el inventario encontrado.
        return inventario;
    }

    // POST: api/Inventarios
    // Registra un nuevo producto en el inventario.
    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<ActionResult<Inventario>> PostInventario(
        Inventario inventario)
    {
        // Establece el ID en cero para que la base de datos lo genere.
        inventario.IdInventario = 0;

        // Registra automáticamente la fecha y hora de actualización.
        inventario.FechaActualizacion = DateTime.Now;

        // Agrega el inventario al contexto.
        _context.Inventarios.Add(inventario);

        // Guarda el nuevo registro en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta indicando que el inventario fue creado.
        return CreatedAtAction(
            // Indica la acción utilizada para consultar el inventario.
            nameof(GetInventario),

            // Envía el ID del inventario creado.
            new { id = inventario.IdInventario },

            // Devuelve los datos del inventario registrado.
            inventario
        );
    }

    // PUT: api/Inventarios/5
    // Actualiza un registro de inventario existente.
    [Authorize(Roles = "Administrador")]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutInventario(
        int id,
        Inventario inventario)
    {
        // Busca el inventario existente mediante su identificador.
        var inventarioActual = await _context.Inventarios.FindAsync(id);

        // Verifica si el inventario no existe.
        if (inventarioActual == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Actualiza el producto asociado al inventario.
        inventarioActual.IdProducto = inventario.IdProducto;

        // Actualiza la cantidad disponible del producto.
        inventarioActual.CantidadDisponible =
            inventario.CantidadDisponible;

        // Actualiza automáticamente la fecha de modificación.
        inventarioActual.FechaActualizacion = DateTime.Now;

        // Guarda los cambios realizados en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se actualizó correctamente.
        return NoContent();
    }

    // DELETE: api/Inventarios/5
    // Elimina un registro de inventario existente.
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInventario(int id)
    {
        // Busca el inventario mediante su identificador.
        var inventario = await _context.Inventarios.FindAsync(id);

        // Verifica si el inventario no existe.
        if (inventario == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Marca el inventario para ser eliminado.
        _context.Inventarios.Remove(inventario);

        // Guarda la eliminación en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se eliminó correctamente.
        return NoContent();
    }
}