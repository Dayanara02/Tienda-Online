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

// Define la ruta principal para acceder al controlador.
[Route("api/[controller]")]

// Indica que esta clase funciona como un controlador de API.
[ApiController]
public class FamiliaProductosController : ControllerBase
{
    // Guarda el contexto utilizado para acceder a la base de datos.
    private readonly TiendaOnlineContext _context;

    // Constructor que recibe el contexto mediante inyección de dependencias.
    public FamiliaProductosController(TiendaOnlineContext context)
    {
        // Asigna el contexto recibido a la variable privada.
        _context = context;
    }

    // GET: api/FamiliaProductos
    // Obtiene todas las familias de productos registradas.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FamiliaProducto>>> GetFamilias()
    {
        // Consulta y devuelve todas las familias de productos.
        return await _context.FamiliaProductos.ToListAsync();
    }

    // GET: api/FamiliaProductos/5
    // Obtiene una familia específica mediante su ID.
    [HttpGet("{id}")]
    public async Task<ActionResult<FamiliaProducto>> GetFamilia(int id)
    {
        // Busca la familia utilizando su identificador.
        var familia = await _context.FamiliaProductos.FindAsync(id);

        // Verifica si la familia no existe.
        if (familia == null)
            return NotFound();

        // Devuelve la familia encontrada.
        return familia;
    }

    // POST: api/FamiliaProductos
    // Registra una nueva familia de productos.
    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<ActionResult<FamiliaProducto>> PostFamilia(
        FamiliaProducto familia)
    {
        // Establece el ID en cero para que la base de datos lo genere.
        familia.IdFamilia = 0;

        // Agrega la nueva familia al contexto.
        _context.FamiliaProductos.Add(familia);

        // Guarda la nueva familia en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta indicando que la familia fue creada.
        return CreatedAtAction(
            // Indica la acción utilizada para consultar la familia.
            nameof(GetFamilia),

            // Envía el ID de la familia creada.
            new { id = familia.IdFamilia },

            // Devuelve los datos de la familia registrada.
            familia
        );
    }

    // PUT: api/FamiliaProductos/5
    // Actualiza una familia de productos existente.
    [Authorize(Roles = "Administrador")]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutFamilia(
        int id,
        FamiliaProducto familia)
    {
        // Verifica que el ID de la URL coincida con el ID recibido.
        if (id != familia.IdFamilia)
            return BadRequest();

        // Busca la familia existente mediante su identificador.
        var existente = await _context.FamiliaProductos.FindAsync(id);

        // Verifica si la familia no existe.
        if (existente == null)
            return NotFound();

        // Actualiza el nombre de la familia.
        existente.Nombre = familia.Nombre;

        // Actualiza la descripción de la familia.
        existente.Descripcion = familia.Descripcion;

        // Actualiza el estado de la familia.
        existente.Estado = familia.Estado;

        // Guarda los cambios realizados en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se actualizó correctamente.
        return NoContent();
    }

    // DELETE: api/FamiliaProductos/5
    // Elimina una familia de productos existente.
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFamilia(int id)
    {
        // Busca la familia mediante su identificador.
        var familia = await _context.FamiliaProductos.FindAsync(id);

        // Verifica si la familia no existe.
        if (familia == null)
            return NotFound();

        // Marca la familia para ser eliminada.
        _context.FamiliaProductos.Remove(familia);

        // Guarda la eliminación en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se eliminó correctamente.
        return NoContent();
    }
}