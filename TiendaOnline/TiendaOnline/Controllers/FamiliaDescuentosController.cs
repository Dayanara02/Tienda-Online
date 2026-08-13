// Importa las funcionalidades necesarias para crear controladores y respuestas HTTP.
using Microsoft.AspNetCore.Mvc;

// Importa Entity Framework Core para realizar consultas a la base de datos.
using Microsoft.EntityFrameworkCore;

// Importa las entidades utilizadas por el controlador.
using TiendaOnline.Dominio.Entidades;

// Importa el contexto de la base de datos de la tienda.
using TiendaOnline.AccesoDatos.Context;

// Importa las herramientas para controlar el acceso mediante roles.
using Microsoft.AspNetCore.Authorization;

// Define el espacio de nombres donde se encuentra el controlador.
namespace TiendaOnline.API.Controllers
{
    // Define la ruta principal para acceder al controlador.
    [Route("api/[controller]")]

    // Indica que esta clase funciona como un controlador de API.
    [ApiController]
    public class FamiliaDescuentosController : ControllerBase
    {
        // Guarda el contexto utilizado para acceder a la base de datos.
        private readonly TiendaOnlineContext _context;

        // Constructor que recibe el contexto mediante inyección de dependencias.
        public FamiliaDescuentosController(TiendaOnlineContext context)
        {
            // Asigna el contexto recibido a la variable privada.
            _context = context;
        }

        // GET: api/FamiliaDescuentos
        // Obtiene todas las relaciones entre familias de productos y descuentos.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetFamiliaDescuentos()
        {
            // Consulta las familias y carga los descuentos relacionados.
            var datos = await _context.FamiliaProductos
                .Include(f => f.IdDescuentos)

                // Obtiene cada descuento relacionado con su familia.
                .SelectMany(f => f.IdDescuentos.Select(d => new
                {
                    // Guarda el ID de la familia.
                    IdFamilia = f.IdFamilia,

                    // Guarda el ID del descuento.
                    IdDescuento = d.IdDescuento
                }))

                // Ejecuta la consulta y obtiene los resultados.
                .ToListAsync();

            // Devuelve los datos encontrados.
            return Ok(datos);
        }

        // GET: api/FamiliaDescuentos/1/2
        // Busca una relación específica entre una familia y un descuento.
        [HttpGet("{idFamilia}/{idDescuento}")]
        public async Task<ActionResult<object>> GetFamiliaDescuento(
            int idFamilia,
            int idDescuento)
        {
            // Busca la familia y carga sus descuentos relacionados.
            var familia = await _context.FamiliaProductos
                .Include(f => f.IdDescuentos)

                // Busca la familia mediante su identificador.
                .FirstOrDefaultAsync(f => f.IdFamilia == idFamilia);

            // Verifica si la familia no existe.
            if (familia == null)
                return NotFound();

            // Comprueba si el descuento está relacionado con la familia.
            var existe = familia.IdDescuentos
                .Any(d => d.IdDescuento == idDescuento);

            // Si la relación no existe, devuelve una respuesta 404.
            if (!existe)
                return NotFound();

            // Devuelve los identificadores de la relación encontrada.
            return Ok(new
            {
                IdFamilia = idFamilia,
                IdDescuento = idDescuento
            });
        }

        // POST: api/FamiliaDescuentos
        // Permite asociar un descuento con una familia de productos.
        [HttpPost]

        // Solo permite realizar esta operación a un Administrador.
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PostFamiliaDescuento(
            int idFamilia,
            int idDescuento)
        {
            // Busca la familia y carga sus descuentos relacionados.
            var familia = await _context.FamiliaProductos
                .Include(f => f.IdDescuentos)

                // Busca la familia mediante su identificador.
                .FirstOrDefaultAsync(f => f.IdFamilia == idFamilia);

            // Busca el descuento utilizando su identificador.
            var descuento = await _context.Descuentos
                .FindAsync(idDescuento);

            // Verifica que la familia y el descuento existan.
            if (familia == null || descuento == null)
                return NotFound();

            // Comprueba si la relación ya existe.
            if (familia.IdDescuentos.Any(d => d.IdDescuento == idDescuento))
                return Conflict("La relación ya existe.");

            // Agrega el descuento a la familia.
            familia.IdDescuentos.Add(descuento);

            // Guarda la nueva relación en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve un mensaje indicando que la asociación fue exitosa.
            return Ok(new
            {
                mensaje = "Descuento asociado correctamente.",
                idFamilia,
                idDescuento
            });
        }

        // DELETE: api/FamiliaDescuentos/1/2
        // Elimina la relación entre una familia y un descuento.
        [HttpDelete("{idFamilia}/{idDescuento}")]

        // Solo permite realizar esta operación a un Administrador.
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteFamiliaDescuento(
            int idFamilia,
            int idDescuento)
        {
            // Busca la familia y carga sus descuentos relacionados.
            var familia = await _context.FamiliaProductos
                .Include(f => f.IdDescuentos)

                // Busca la familia mediante su identificador.
                .FirstOrDefaultAsync(f => f.IdFamilia == idFamilia);

            // Verifica si la familia no existe.
            if (familia == null)
                return NotFound();

            // Busca el descuento relacionado con la familia.
            var descuento = familia.IdDescuentos
                .FirstOrDefault(d => d.IdDescuento == idDescuento);

            // Verifica si el descuento no está relacionado.
            if (descuento == null)
                return NotFound();

            // Elimina el descuento de la familia.
            familia.IdDescuentos.Remove(descuento);

            // Guarda los cambios realizados en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve una respuesta 204 indicando que se eliminó la relación.
            return NoContent();
        }
    }
}