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
public class MovimientoInventariosController : ControllerBase
{
    // Guarda el contexto utilizado para acceder a la base de datos.
    private readonly TiendaOnlineContext _context;

    // Constructor que recibe el contexto mediante inyección de dependencias.
    public MovimientoInventariosController(
        TiendaOnlineContext context)
    {
        // Asigna el contexto recibido a la variable privada.
        _context = context;
    }

    // GET: api/MovimientoInventarios
    // Obtiene todos los movimientos de inventario registrados.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovimientoInventario>>>
        GetMovimientoInventarios()
    {
        // Consulta los movimientos sin realizar seguimiento de cambios.
        return await _context.MovimientoInventarios
            .AsNoTracking()

            // Ordena los movimientos desde el más reciente al más antiguo.
            .OrderByDescending(m => m.FechaMovimiento)

            // Ejecuta la consulta y obtiene los registros.
            .ToListAsync();
    }

    // GET: api/MovimientoInventarios/5
    // Obtiene un movimiento específico mediante su ID.
    [HttpGet("{id}")]
    public async Task<ActionResult<MovimientoInventario>>
        GetMovimientoInventario(int id)
    {
        // Busca el movimiento utilizando su identificador.
        var movimiento =
            await _context.MovimientoInventarios.FindAsync(id);

        // Verifica si el movimiento no existe.
        if (movimiento == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Devuelve el movimiento encontrado.
        return movimiento;
    }

    // POST: api/MovimientoInventarios
    // Registra un nuevo movimiento y actualiza el inventario.
    [HttpPost]
    public async Task<ActionResult<MovimientoInventario>>
        PostMovimientoInventario(
            MovimientoInventario movimiento)
    {
        // Busca el inventario relacionado con el movimiento.
        var inventario = await _context.Inventarios
            .FindAsync(movimiento.IdInventario);

        // Verifica si el inventario no existe.
        if (inventario == null)
        {
            // Devuelve un error indicando que no existe.
            return BadRequest("El inventario no existe.");
        }

        // Comprueba que el usuario asociado exista.
        var usuarioExiste = await _context.Usuarios
            .AnyAsync(u => u.IdUsuario == movimiento.IdUsuario);

        // Verifica si el usuario no existe.
        if (!usuarioExiste)
        {
            // Devuelve un error indicando que no existe.
            return BadRequest("El usuario no existe.");
        }

        // Define los tipos de movimiento permitidos.
        var tiposPermitidos = new[]
        {
            "Entrada",
            "Salida",
            "Ajuste"
        };

        // Verifica que el tipo de movimiento sea válido.
        if (!tiposPermitidos.Contains(movimiento.TipoMovimiento))
        {
            // Devuelve un error indicando los tipos permitidos.
            return BadRequest(
                "El tipo de movimiento debe ser Entrada, Salida o Ajuste."
            );
        }

        // Verifica que la cantidad sea mayor que cero.
        if (movimiento.Cantidad <= 0)
        {
            // Devuelve un error si la cantidad no es válida.
            return BadRequest(
                "La cantidad debe ser mayor que cero."
            );
        }

        // Comprueba si el movimiento corresponde a una salida.
        if (movimiento.TipoMovimiento == "Salida")
        {
            // Verifica que exista suficiente inventario disponible.
            if (inventario.CantidadDisponible <
                movimiento.Cantidad)
            {
                // Devuelve un error si no hay suficiente cantidad.
                return BadRequest(
                    "No existe suficiente cantidad disponible."
                );
            }

            // Reduce la cantidad disponible según la salida.
            inventario.CantidadDisponible -=
                movimiento.Cantidad;
        }

        // Comprueba si el movimiento corresponde a una entrada.
        else if (movimiento.TipoMovimiento == "Entrada")
        {
            // Aumenta la cantidad disponible según la entrada.
            inventario.CantidadDisponible +=
                movimiento.Cantidad;
        }

        // Comprueba si el movimiento corresponde a un ajuste.
        else if (movimiento.TipoMovimiento == "Ajuste")
        {
            // Establece la cantidad del inventario según el ajuste.
            inventario.CantidadDisponible =
                movimiento.Cantidad;
        }

        // Actualiza la fecha de modificación del inventario.
        inventario.FechaActualizacion = DateTime.Now;

        // Establece el ID en cero para que la base de datos lo genere.
        movimiento.IdMovimiento = 0;

        // Registra automáticamente la fecha y hora del movimiento.
        movimiento.FechaMovimiento = DateTime.Now;

        // Agrega el movimiento al contexto.
        _context.MovimientoInventarios.Add(movimiento);

        // Guarda el movimiento y los cambios del inventario.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta indicando que el movimiento fue creado.
        return CreatedAtAction(
            // Indica la acción utilizada para consultar el movimiento.
            nameof(GetMovimientoInventario),

            // Envía el ID del movimiento creado.
            new { id = movimiento.IdMovimiento },

            // Devuelve los datos del movimiento registrado.
            movimiento
        );
    }
}