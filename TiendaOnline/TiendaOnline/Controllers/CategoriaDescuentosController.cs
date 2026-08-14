// Importa las funcionalidades necesarias para crear controladores y respuestas HTTP.
using Microsoft.AspNetCore.Mvc;

// Importa Entity Framework Core para realizar consultas a la base de datos.
using Microsoft.EntityFrameworkCore;

// Importa el contexto de la base de datos de la tienda.
using TiendaOnline.AccesoDatos.Context;

// Importa las entidades utilizadas por el controlador.
using TiendaOnline.Dominio.Entidades;

// Importa las herramientas para controlar el acceso según los roles.
using Microsoft.AspNetCore.Authorization;

// Define el espacio de nombres donde se encuentra el controlador.
namespace TiendaOnline.API.Controllers
{
    // Define la ruta principal de las solicitudes del controlador.
    [Route("api/[controller]")]

    // Indica que esta clase funciona como un controlador de API.
    [ApiController]
    public class CategoriaDescuentosController : ControllerBase
    {
        // Guarda el contexto utilizado para acceder a la base de datos.
        private readonly TiendaOnlineContext _context;

        // Constructor que recibe el contexto mediante inyección de dependencias.
        public CategoriaDescuentosController(TiendaOnlineContext context)
        {
            // Asigna el contexto recibido a la variable privada.
            _context = context;
        }

        // GET: api/CategoriaDescuentos
        // Obtiene todas las relaciones entre categorías y descuentos.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCategoriaDescuentos()
        {
            // Consulta las categorías y carga sus descuentos relacionados.
            var datos = await _context.Categoria
                .Include(c => c.IdDescuentos)

                // Obtiene cada descuento relacionado con su categoría.
                .SelectMany(c => c.IdDescuentos.Select(d => new
                {
                    // Guarda el ID de la categoría.
                    IdCategoria = c.IdCategoria,

                    // Guarda el ID del descuento.
                    IdDescuento = d.IdDescuento
                }))

                // Ejecuta la consulta y obtiene los resultados.
                .ToListAsync();

            // Devuelve los datos obtenidos con una respuesta correcta.
            return Ok(datos);
        }

        // GET: api/CategoriaDescuentos/1/2
        // Busca una relación específica entre categoría y descuento.
        [HttpGet("{idCategoria}/{idDescuento}")]
        public async Task<ActionResult<object>> GetCategoriaDescuento(
            int idCategoria,
            int idDescuento)
        {
            // Busca la categoría y carga sus descuentos relacionados.
            var categoria = await _context.Categoria
                .Include(c => c.IdDescuentos)

                // Busca la categoría utilizando su identificador.
                .FirstOrDefaultAsync(c => c.IdCategoria == idCategoria);

            // Verifica si la categoría no existe.
            if (categoria == null)
                return NotFound();

            // Comprueba si el descuento está relacionado con la categoría.
            var existe = categoria.IdDescuentos
                .Any(d => d.IdDescuento == idDescuento);

            // Si la relación no existe, devuelve un error 404.
            if (!existe)
                return NotFound();

            // Devuelve los identificadores de la relación encontrada.
            return Ok(new
            {
                IdCategoria = idCategoria,
                IdDescuento = idDescuento
            });
        }


        // POST: api/CategoriaDescuentos
        // Permite asociar un descuento con una categoría.
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> PostCategoriaDescuento(
            [FromBody] CategoriaDescuentoRequest request)
        {
            // Busca la categoría y carga sus descuentos relacionados.
            var categoria = await _context.Categoria
                .Include(c => c.IdDescuentos)

                // Busca la categoría indicada en la solicitud.
                .FirstOrDefaultAsync(c => c.IdCategoria == request.IdCategoria);

            // Busca el descuento utilizando su identificador.
            var descuento = await _context.Descuentos
                .FindAsync(request.IdDescuento);

            // Verifica que la categoría y el descuento existan.
            if (categoria == null || descuento == null)
                return NotFound("La categoría o el descuento no existe.");

            // Comprueba si la relación ya existe.
            if (categoria.IdDescuentos.Any(d => d.IdDescuento == request.IdDescuento))
                return Conflict("La relación ya existe.");

            // Agrega el descuento a la categoría.
            categoria.IdDescuentos.Add(descuento);

            // Guarda la nueva relación en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve un mensaje confirmando la asociación.
            return Ok(new
            {
                mensaje = "Descuento asociado correctamente.",
                idCategoria = request.IdCategoria,
                idDescuento = request.IdDescuento
            });

        }

        // DELETE: api/CategoriaDescuentos/1/2
        // Elimina la relación entre una categoría y un descuento.
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{idCategoria}/{idDescuento}")]
        public async Task<IActionResult> DeleteCategoriaDescuento(
            int idCategoria,
            int idDescuento)
        {
            // Busca la categoría y carga sus descuentos relacionados.
            var categoria = await _context.Categoria
                .Include(c => c.IdDescuentos)

                // Busca la categoría mediante su identificador.
                .FirstOrDefaultAsync(c => c.IdCategoria == idCategoria);

            // Verifica si la categoría no existe.
            if (categoria == null)
                return NotFound();

            // Busca el descuento relacionado con la categoría.
            var descuento = categoria.IdDescuentos
                .FirstOrDefault(d => d.IdDescuento == idDescuento);

            // Verifica si el descuento no está relacionado.
            if (descuento == null)
                return NotFound();

            // Elimina el descuento de la categoría.
            categoria.IdDescuentos.Remove(descuento);

            // Guarda los cambios en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve una respuesta indicando que no hay contenido.
            return NoContent();
        }

        // Clase utilizada para recibir los datos de la relación.
        public class CategoriaDescuentoRequest
        {
            // Guarda el identificador de la categoría.
            public int IdCategoria { get; set; }

            // Guarda el identificador del descuento.
            public int IdDescuento { get; set; }
        }
    }
}