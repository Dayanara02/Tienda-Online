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
namespace TiendaOnline.API.Controllers
{
    // Define la ruta principal para acceder al controlador.
    [Route("api/[controller]")]

    // Indica que esta clase funciona como un controlador de API.
    [ApiController]
    public class ImpuestosController : ControllerBase
    {
        // Guarda el contexto utilizado para acceder a la base de datos.
        private readonly TiendaOnlineContext _context;

        // Constructor que recibe el contexto mediante inyección de dependencias.
        public ImpuestosController(TiendaOnlineContext context)
        {
            // Asigna el contexto recibido a la variable privada.
            _context = context;
        }

        // GET: api/Impuestos
        // Obtiene todos los impuestos registrados.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Impuesto>>> GetImpuestos()
        {
            // Consulta todos los impuestos de la base de datos.
            return await _context.Impuestos.ToListAsync();
        }

        // GET: api/Impuestos/5
        // Obtiene un impuesto específico mediante su ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<Impuesto>> GetImpuesto(int id)
        {
            // Busca el impuesto utilizando su identificador.
            var impuesto = await _context.Impuestos.FindAsync(id);

            // Verifica si el impuesto no existe.
            if (impuesto == null)
                return NotFound();

            // Devuelve el impuesto encontrado.
            return impuesto;
        }

        // POST: api/Impuestos
        // Registra un nuevo impuesto.
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<ActionResult<Impuesto>> PostImpuesto(
            Impuesto impuesto)
        {
            // Establece el ID en cero para que la base de datos lo genere.
            impuesto.IdImpuesto = 0;

            // Agrega el impuesto al contexto.
            _context.Impuestos.Add(impuesto);

            // Guarda el nuevo impuesto en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve una respuesta indicando que el impuesto fue creado.
            return CreatedAtAction(
                // Indica la acción utilizada para consultar el impuesto.
                nameof(GetImpuesto),

                // Envía el ID del impuesto creado.
                new { id = impuesto.IdImpuesto },

                // Devuelve los datos del impuesto registrado.
                impuesto
            );
        }

        // PUT: api/Impuestos/5
        // Actualiza un impuesto existente.
        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutImpuesto(
            int id,
            Impuesto impuesto)
        {
            // Verifica que el ID de la URL coincida con el ID recibido.
            if (id != impuesto.IdImpuesto)
                return BadRequest();

            // Busca el impuesto existente mediante su identificador.
            var existente = await _context.Impuestos.FindAsync(id);

            // Verifica si el impuesto no existe.
            if (existente == null)
                return NotFound();

            // Actualiza el nombre del impuesto.
            existente.Nombre = impuesto.Nombre;

            // Actualiza el porcentaje del impuesto.
            existente.Porcentaje = impuesto.Porcentaje;

            // Actualiza el estado del impuesto.
            existente.Estado = impuesto.Estado;

            // Guarda los cambios realizados en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve una respuesta 204 indicando que se actualizó correctamente.
            return NoContent();
        }

        // DELETE: api/Impuestos/5
        // Elimina un impuesto existente.
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImpuesto(int id)
        {
            // Busca el impuesto mediante su identificador.
            var impuesto = await _context.Impuestos.FindAsync(id);

            // Verifica si el impuesto no existe.
            if (impuesto == null)
                return NotFound();

            // Marca el impuesto para ser eliminado.
            _context.Impuestos.Remove(impuesto);

            // Guarda la eliminación en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve una respuesta 204 indicando que se eliminó correctamente.
            return NoContent();
        }
    }
}