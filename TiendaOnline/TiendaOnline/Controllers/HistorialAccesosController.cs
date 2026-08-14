// Importa las funcionalidades necesarias para crear controladores y respuestas HTTP.
using Microsoft.AspNetCore.Mvc;

// Importa Entity Framework Core para realizar consultas a la base de datos.
using Microsoft.EntityFrameworkCore;

// Importa el contexto de la base de datos de la tienda.
using TiendaOnline.AccesoDatos.Context;

// Importa las herramientas para controlar la autorización de los usuarios.
using Microsoft.AspNetCore.Authorization;

// Importa las entidades utilizadas por el controlador.
using TiendaOnline.Dominio.Entidades;

// Define el espacio de nombres donde se encuentra el controlador.
namespace TiendaOnline.API.Controllers
{
    // Define la ruta principal para acceder al controlador.
    [Route("api/[controller]")]

    // Indica que esta clase funciona como un controlador de API.
    [ApiController]
    public class HistorialAccesosController : ControllerBase
    {
        // Guarda el contexto utilizado para acceder a la base de datos.
        private readonly TiendaOnlineContext _context;

        // Constructor que recibe el contexto mediante inyección de dependencias.
        public HistorialAccesosController(TiendaOnlineContext context)
        {
            // Asigna el contexto recibido a la variable privada.
            _context = context;
        }

        // GET: api/HistorialAccesos
        // Obtiene todos los registros del historial de accesos.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistorialAcceso>>> GetHistorialAccesos()
        {
            // Consulta todos los registros del historial.
            return await _context.HistorialAccesos.ToListAsync();
        }

        // GET: api/HistorialAccesos/5
        // Obtiene un registro específico mediante su ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<HistorialAcceso>> GetHistorialAcceso(int id)
        {
            // Busca el registro utilizando su identificador.
            var historial = await _context.HistorialAccesos.FindAsync(id);

            // Verifica si el registro no existe.
            if (historial == null)
                return NotFound();

            // Devuelve el registro encontrado.
            return historial;
        }

        // POST: api/HistorialAccesos
        // Registra un nuevo acceso en el historial.
        [HttpPost]
        public async Task<ActionResult<HistorialAcceso>> PostHistorialAcceso(
            HistorialAcceso historial)
        {
            // Establece el ID en cero para que la base de datos lo genere.
            historial.IdHistorialAcceso = 0;

            // Agrega el registro al contexto.
            _context.HistorialAccesos.Add(historial);

            // Guarda el nuevo registro en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve una respuesta indicando que el registro fue creado.
            return CreatedAtAction(
                // Indica la acción utilizada para consultar el registro.
                nameof(GetHistorialAcceso),

                // Envía el ID del historial creado.
                new { id = historial.IdHistorialAcceso },

                // Devuelve los datos del registro creado.
                historial
            );
        }

        // PUT: api/HistorialAccesos/5
        // Actualiza un registro existente del historial.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHistorialAcceso(
            int id,
            HistorialAcceso historial)
        {
            // Verifica que el ID de la URL coincida con el ID recibido.
            if (id != historial.IdHistorialAcceso)
                return BadRequest();

            // Busca el registro existente mediante su identificador.
            var existente = await _context.HistorialAccesos.FindAsync(id);

            // Verifica si el registro no existe.
            if (existente == null)
                return NotFound();

            // Actualiza el usuario asociado al acceso.
            existente.IdUsuario = historial.IdUsuario;

            // Actualiza la fecha y hora del acceso.
            existente.FechaAcceso = historial.FechaAcceso;

            // Actualiza la dirección IP registrada.
            existente.DireccionIp = historial.DireccionIp;

            // Actualiza si el acceso fue exitoso.
            existente.Exitoso = historial.Exitoso;

            // Guarda los cambios realizados en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve una respuesta 204 indicando que se actualizó correctamente.
            return NoContent();
        }

        // DELETE: api/HistorialAccesos/5
        // Elimina un registro del historial de accesos.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistorialAcceso(int id)
        {
            // Busca el registro mediante su identificador.
            var historial = await _context.HistorialAccesos.FindAsync(id);

            // Verifica si el registro no existe.
            if (historial == null)
                return NotFound();

            // Marca el registro para ser eliminado.
            _context.HistorialAccesos.Remove(historial);

            // Guarda la eliminación en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve una respuesta 204 indicando que se eliminó correctamente.
            return NoContent();
        }
    }
}