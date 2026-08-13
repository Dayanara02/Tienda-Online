// Importa las funcionalidades para controlar la autorización de los usuarios.
using Microsoft.AspNetCore.Authorization;

// Importa las herramientas necesarias para crear controladores y respuestas HTTP.
using Microsoft.AspNetCore.Mvc;

// Importa Entity Framework Core para trabajar con la base de datos.
using Microsoft.EntityFrameworkCore;

// Importa el contexto de la base de datos de la tienda.
using TiendaOnline.AccesoDatos.Context;

// Importa las entidades utilizadas por el controlador.
using TiendaOnline.Dominio.Entidades;

// Define el espacio de nombres donde se encuentra este controlador.
namespace TiendaOnline.API.Controllers;

// Indica que esta clase es un controlador de API.
[ApiController]

// Define la ruta base para acceder al controlador.
[Route("api/[controller]")]

// Indica que todas las acciones requieren un usuario autenticado.
[Authorize]
public class CarritosController : ControllerBase
{
    // Almacena el contexto para acceder a la base de datos.
    private readonly TiendaOnlineContext _context;

    // Constructor que recibe el contexto mediante inyección de dependencias.
    public CarritosController(TiendaOnlineContext context)
    {
        // Guarda el contexto recibido para utilizarlo en el controlador.
        _context = context;
    }

    // GET: api/Carritos
    // Obtiene todos los carritos registrados.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Carrito>>> GetCarritos()
    {
        // Consulta los carritos sin realizar seguimiento de cambios.
        return await _context.Carritos
            // Mejora el rendimiento al tratarse de una consulta de solo lectura.
            .AsNoTracking()
            // Ejecuta la consulta y obtiene todos los registros.
            .ToListAsync();
    }

    // GET: api/Carritos/5
    // Obtiene un carrito específico utilizando su ID.
    [HttpGet("{id}")]
    public async Task<ActionResult<Carrito>> GetCarrito(int id)
    {
        // Busca el carrito por su identificador.
        var carrito = await _context.Carritos.FindAsync(id);

        // Verifica si el carrito no existe.
        if (carrito == null)
        {
            // Devuelve una respuesta 404 indicando que no fue encontrado.
            return NotFound();
        }

        // Devuelve el carrito encontrado.
        return carrito;
    }

    // POST: api/Carritos
    // Crea un nuevo carrito en la base de datos.
    [HttpPost]
    public async Task<ActionResult<Carrito>> PostCarrito(
        Carrito carrito)
    {
        // Establece el ID en cero para que la base de datos lo genere.
        carrito.IdCarrito = 0;

        // Registra la fecha y hora de creación del carrito.
        carrito.FechaCreacion = DateTime.Now;

        // Verifica si el estado no fue indicado.
        if (string.IsNullOrWhiteSpace(carrito.Estado))
        {
            // Establece el carrito como activo por defecto.
            carrito.Estado = "Activo";
        }

        // Agrega el nuevo carrito al contexto.
        _context.Carritos.Add(carrito);

        // Guarda los cambios realizados en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 201 indicando que el carrito fue creado.
        return CreatedAtAction(
            // Indica la acción utilizada para consultar el carrito creado.
            nameof(GetCarrito),

            // Envía el ID del carrito creado en la URL.
            new { id = carrito.IdCarrito },

            // Devuelve los datos del carrito creado.
            carrito
        );
    }

    // PUT: api/Carritos/5
    // Actualiza los datos de un carrito existente.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCarrito(
        int id,
        Carrito carrito)
    {
        // Busca el carrito existente utilizando el ID recibido.
        var carritoActual = await _context.Carritos.FindAsync(id);

        // Verifica si el carrito no existe.
        if (carritoActual == null)
        {
            // Devuelve una respuesta 404 indicando que no fue encontrado.
            return NotFound();
        }

        // Actualiza el usuario asociado al carrito.
        carritoActual.IdUsuario = carrito.IdUsuario;

        // Actualiza el estado del carrito.
        carritoActual.Estado = carrito.Estado;

        // Guarda los cambios realizados.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que la actualización fue exitosa.
        return NoContent();
    }

    // DELETE: api/Carritos/5
    // Elimina un carrito existente.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCarrito(int id)
    {
        // Busca el carrito por su identificador.
        var carrito = await _context.Carritos.FindAsync(id);

        // Verifica si el carrito no existe.
        if (carrito == null)
        {
            // Devuelve una respuesta 404 indicando que no fue encontrado.
            return NotFound();
        }

        // Marca el carrito para ser eliminado de la base de datos.
        _context.Carritos.Remove(carrito);

        // Guarda la eliminación en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que la eliminación fue exitosa.
        return NoContent();
    }
}