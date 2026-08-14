// Permite crear controladores API.
using Microsoft.AspNetCore.Mvc;

// Permite consultar la base de datos.
using Microsoft.EntityFrameworkCore;

// Importa el contexto principal.
using TiendaOnline.AccesoDatos.Context;

// Importa la entidad Descuento.
using TiendaOnline.Dominio.Entidades;

// Define el espacio de nombres.
namespace TiendaOnline.API.Controllers;

// Define la ruta principal.
[Route("api/[controller]")]

// Indica que es un controlador API.
[ApiController]
public class DescuentosController : ControllerBase
{
    // Guarda el contexto de la base de datos.
    private readonly TiendaOnlineContext _context;

    // Recibe el contexto mediante inyección.
    public DescuentosController(
        TiendaOnlineContext context)
    {
        // Guarda el contexto recibido.
        _context = context;
    }

    // Obtiene todos los descuentos activos.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Descuento>>>
        GetDescuentos()
    {
        // Consulta los descuentos disponibles.
        var descuentos =
            await _context.Descuentos
                .AsNoTracking()
                .Where(
                    descuento =>
                        descuento.Estado
                )
                .OrderBy(
                    descuento =>
                        descuento.CantidadMinima
                )
                .ToListAsync();

        // Devuelve la lista.
        return Ok(
            descuentos
        );
    }

    // Obtiene un descuento por id.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Descuento>>
        GetDescuento(
            int id)
    {
        // Busca un descuento activo.
        var descuento =
            await _context.Descuentos
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    descuento =>
                        descuento.IdDescuento == id &&
                        descuento.Estado
                );

        // Verifica que exista.
        if (descuento == null)
        {
            // Devuelve error 404.
            return NotFound(
                "El descuento no existe."
            );
        }

        // Devuelve el descuento.
        return Ok(
            descuento
        );
    }

    // Obtiene el mejor descuento aplicable.
    [HttpGet("aplicable/{cantidad:int}")]
    public async Task<ActionResult<Descuento>>
        GetDescuentoAplicable(
            int cantidad)
    {
        // Valida la cantidad recibida.
        if (cantidad < 0)
        {
            // Devuelve error 400.
            return BadRequest(
                "La cantidad no puede ser negativa."
            );
        }

        // Busca el descuento más alto disponible.
        var descuento =
            await _context.Descuentos
                .AsNoTracking()
                .Where(
                    descuento =>
                        descuento.Estado &&
                        cantidad >=
                        descuento.CantidadMinima
                )
                .OrderByDescending(
                    descuento =>
                        descuento.Porcentaje
                )
                .FirstOrDefaultAsync();

        // Verifica si aplica algún descuento.
        if (descuento == null)
        {
            // Informa que no aplica descuento.
            return NotFound(
                "La compra todavía no cumple con la cantidad mínima para obtener un descuento."
            );
        }

        // Devuelve el descuento aplicable.
        return Ok(
            descuento
        );
    }
}