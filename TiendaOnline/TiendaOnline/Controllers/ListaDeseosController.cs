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

// Permite el acceso únicamente a usuarios autenticados.
[Authorize]

// Indica que esta clase funciona como un controlador de API.
[ApiController]

// Define la ruta principal para acceder al controlador.
[Route("api/[controller]")]
public class ListaDeseosController : ControllerBase
{
    // Guarda el contexto utilizado para acceder a la base de datos.
    private readonly TiendaOnlineContext _context;

    // Constructor que recibe el contexto mediante inyección de dependencias.
    public ListaDeseosController(TiendaOnlineContext context)
    {
        // Asigna el contexto recibido a la variable privada.
        _context = context;
    }

    // GET: api/ListaDeseos
    // Obtiene todas las listas de deseos registradas.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ListaDeseo>>> GetListaDeseos()
    {
        // Consulta las listas de deseos sin realizar seguimiento de cambios.
        return await _context.ListaDeseos
            .AsNoTracking()

            // Ejecuta la consulta y obtiene todos los registros.
            .ToListAsync();
    }

    // GET: api/ListaDeseos/5
    // Obtiene una lista de deseos específica mediante su ID.
    [HttpGet("{id}")]
    public async Task<ActionResult<ListaDeseo>> GetListaDeseo(int id)
    {
        // Busca la lista de deseos utilizando su identificador.
        var listaDeseo = await _context.ListaDeseos.FindAsync(id);

        // Verifica si la lista de deseos no existe.
        if (listaDeseo == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Devuelve la lista de deseos encontrada.
        return listaDeseo;
    }

    // POST: api/ListaDeseos
    // Registra una nueva lista de deseos.
    [HttpPost]
    public async Task<ActionResult<ListaDeseo>> PostListaDeseo(
        ListaDeseo listaDeseo)
    {
        // Establece el ID en cero para que la base de datos lo genere.
        listaDeseo.IdListaDeseos = 0;

        // Registra automáticamente la fecha de creación.
        listaDeseo.FechaCreacion = DateTime.Now;

        // Agrega la lista de deseos al contexto.
        _context.ListaDeseos.Add(listaDeseo);

        // Guarda la nueva lista en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta indicando que la lista fue creada.
        return CreatedAtAction(
            // Indica la acción utilizada para consultar la lista.
            nameof(GetListaDeseo),

            // Envía el ID de la lista creada.
            new { id = listaDeseo.IdListaDeseos },

            // Devuelve los datos de la lista registrada.
            listaDeseo
        );
    }

    // PUT: api/ListaDeseos/5
    // Actualiza una lista de deseos existente.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutListaDeseo(
        int id,
        ListaDeseo listaDeseo)
    {
        // Busca la lista existente mediante su identificador.
        var listaActual = await _context.ListaDeseos.FindAsync(id);

        // Verifica si la lista no existe.
        if (listaActual == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Actualiza el usuario asociado a la lista de deseos.
        listaActual.IdUsuario = listaDeseo.IdUsuario;

        // Guarda los cambios realizados en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se actualizó correctamente.
        return NoContent();
    }

    // DELETE: api/ListaDeseos/5
    // Elimina una lista de deseos existente.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteListaDeseo(int id)
    {
        // Busca la lista de deseos mediante su identificador.
        var listaDeseo = await _context.ListaDeseos.FindAsync(id);

        // Verifica si la lista no existe.
        if (listaDeseo == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Marca la lista de deseos para ser eliminada.
        _context.ListaDeseos.Remove(listaDeseo);

        // Guarda la eliminación en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se eliminó correctamente.
        return NoContent();
    }
}