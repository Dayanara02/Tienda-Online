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
public class EvaluacionProductosController : ControllerBase
{
    // Guarda el contexto utilizado para acceder a la base de datos.
    private readonly TiendaOnlineContext _context;

    // Constructor que recibe el contexto mediante inyección de dependencias.
    public EvaluacionProductosController(
        TiendaOnlineContext context)
    {
        // Asigna el contexto recibido a la variable privada.
        _context = context;
    }

    // GET: api/EvaluacionProductos
    // Obtiene todas las evaluaciones de productos registradas.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EvaluacionProducto>>>
        GetEvaluacionProductos()
    {
        // Consulta las evaluaciones sin realizar seguimiento de cambios.
        return await _context.EvaluacionProductos
            .AsNoTracking()

            // Ejecuta la consulta y obtiene todos los registros.
            .ToListAsync();
    }

    // GET: api/EvaluacionProductos/5
    // Obtiene una evaluación específica mediante su ID.
    [HttpGet("{id}")]
    public async Task<ActionResult<EvaluacionProducto>>
        GetEvaluacionProducto(int id)
    {
        // Busca la evaluación utilizando su identificador.
        var evaluacion =
            await _context.EvaluacionProductos.FindAsync(id);

        // Verifica si la evaluación no existe.
        if (evaluacion == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Devuelve la evaluación encontrada.
        return evaluacion;
    }

    // POST: api/EvaluacionProductos
    // Registra una nueva evaluación de producto.
    [HttpPost]
    public async Task<ActionResult<EvaluacionProducto>>
        PostEvaluacionProducto(
            EvaluacionProducto evaluacion)
    {
        // Establece el ID en cero para que la base de datos lo genere.
        evaluacion.IdEvaluacion = 0;

        // Registra automáticamente la fecha y hora de la evaluación.
        evaluacion.FechaEvaluacion = DateTime.Now;

        // Agrega la evaluación al contexto.
        _context.EvaluacionProductos.Add(evaluacion);

        // Guarda la nueva evaluación en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta indicando que la evaluación fue creada.
        return CreatedAtAction(
            // Indica la acción utilizada para consultar la evaluación.
            nameof(GetEvaluacionProducto),

            // Envía el ID de la evaluación creada.
            new { id = evaluacion.IdEvaluacion },

            // Devuelve los datos de la evaluación registrada.
            evaluacion
        );
    }

    // PUT: api/EvaluacionProductos/5
    // Actualiza una evaluación de producto existente.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEvaluacionProducto(
        int id,
        EvaluacionProducto evaluacion)
    {
        // Busca la evaluación existente mediante su identificador.
        var evaluacionActual =
            await _context.EvaluacionProductos.FindAsync(id);

        // Verifica si la evaluación no existe.
        if (evaluacionActual == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Actualiza el producto asociado a la evaluación.
        evaluacionActual.IdProducto =
            evaluacion.IdProducto;

        // Actualiza el usuario que realizó la evaluación.
        evaluacionActual.IdUsuario =
            evaluacion.IdUsuario;

        // Actualiza la calificación otorgada al producto.
        evaluacionActual.Calificacion =
            evaluacion.Calificacion;

        // Actualiza el comentario de la evaluación.
        evaluacionActual.Comentario =
            evaluacion.Comentario;

        // Actualiza el estado de la evaluación.
        evaluacionActual.Estado =
            evaluacion.Estado;

        // Guarda los cambios realizados en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se actualizó correctamente.
        return NoContent();
    }

    // DELETE: api/EvaluacionProductos/5
    // Elimina una evaluación de producto existente.
    [HttpDelete("{id}")]
    public async Task<IActionResult>
        DeleteEvaluacionProducto(int id)
    {
        // Busca la evaluación mediante su identificador.
        var evaluacion =
            await _context.EvaluacionProductos.FindAsync(id);

        // Verifica si la evaluación no existe.
        if (evaluacion == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Marca la evaluación para ser eliminada.
        _context.EvaluacionProductos.Remove(evaluacion);

        // Guarda la eliminación en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se eliminó correctamente.
        return NoContent();
    }
}