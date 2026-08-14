// Permite crear controladores y manejar respuestas HTTP.
using Microsoft.AspNetCore.Mvc;

// Permite trabajar con Entity Framework Core.
using Microsoft.EntityFrameworkCore;

// Importa el contexto de la base de datos.
using TiendaOnline.AccesoDatos.Context;

// Permite controlar el acceso mediante roles.
using Microsoft.AspNetCore.Authorization;

namespace TiendaOnline.API.Controllers
{
    // Define la ruta base: api/ProductoDescuentos.
    [Route("api/[controller]")]

    // Indica que es un controlador de API.
    [ApiController]
    public class ProductoDescuentosController : ControllerBase
    {
        // Contexto utilizado para acceder a la base de datos.
        private readonly TiendaOnlineContext _context;

        // Constructor del controlador.
        public ProductoDescuentosController(TiendaOnlineContext context)
        {
            // Guarda el contexto recibido.
            _context = context;
        }

        // =========================================================
        // OBTENER TODAS LAS RELACIONES
        // =========================================================

        // GET: api/ProductoDescuentos
        // Obtiene los descuentos asociados a cada producto.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>>
            GetProductoDescuentos()
        {
            // Consulta los productos y sus descuentos relacionados.
            var datos = await _context.Productos
                .Include(p => p.IdDescuentos)

                // Convierte cada relación en un objeto sencillo.
                .SelectMany(
                    p => p.IdDescuentos.Select(
                        d => new
                        {
                            // Identificador del producto.
                            IdProducto = p.IdProducto,

                            // Identificador del descuento.
                            IdDescuento = d.IdDescuento
                        }
                    )
                )
                .ToListAsync();

            // Devuelve las relaciones encontradas.
            return Ok(datos);
        }

        // =========================================================
        // OBTENER UNA RELACIÓN
        // =========================================================

        // GET: api/ProductoDescuentos/1/2
        // Consulta si un descuento está asociado a un producto.
        [HttpGet("{idProducto}/{idDescuento}")]
        public async Task<ActionResult<object>>
            GetProductoDescuento(
                int idProducto,
                int idDescuento)
        {
            // Busca el producto y carga sus descuentos.
            var producto = await _context.Productos
                .Include(p => p.IdDescuentos)
                .FirstOrDefaultAsync(
                    p => p.IdProducto == idProducto
                );

            // Comprueba que el producto exista.
            if (producto == null)
                return NotFound();

            // Comprueba si el descuento está relacionado
            // con el producto.
            var existe = producto.IdDescuentos
                .Any(d => d.IdDescuento == idDescuento);

            // Si la relación no existe, devuelve 404.
            if (!existe)
                return NotFound();

            // Devuelve los identificadores de la relación.
            return Ok(new
            {
                IdProducto = idProducto,
                IdDescuento = idDescuento
            });
        }

        // =========================================================
        // ASOCIAR DESCUENTO A PRODUCTO
        // =========================================================

        // POST: api/ProductoDescuentos
        // Solo los Administradores pueden crear relaciones.
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PostProductoDescuento(
            int idProducto,
            int idDescuento)
        {
            // Busca el producto y sus descuentos actuales.
            var producto = await _context.Productos
                .Include(p => p.IdDescuentos)
                .FirstOrDefaultAsync(
                    p => p.IdProducto == idProducto
                );

            // Busca el descuento que se desea asociar.
            var descuento = await _context.Descuentos
                .FindAsync(idDescuento);

            // Comprueba que ambos registros existan.
            if (producto == null || descuento == null)
                return NotFound();

            // Evita crear una relación duplicada.
            if (
                producto.IdDescuentos
                    .Any(d => d.IdDescuento == idDescuento)
            )
            {
                return Conflict(
                    "La relación ya existe."
                );
            }

            // Agrega el descuento al producto.
            producto.IdDescuentos.Add(descuento);

            // Guarda la nueva relación.
            await _context.SaveChangesAsync();

            // Devuelve un mensaje de confirmación.
            return Ok(new
            {
                mensaje = "Descuento asociado correctamente.",
                idProducto,
                idDescuento
            });
        }

        // =========================================================
        // ELIMINAR RELACIÓN
        // =========================================================

        // DELETE: api/ProductoDescuentos/1/2
        // Solo los Administradores pueden eliminar relaciones.
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{idProducto}/{idDescuento}")]
        public async Task<IActionResult> DeleteProductoDescuento(
            int idProducto,
            int idDescuento)
        {
            // Busca el producto y sus descuentos asociados.
            var producto = await _context.Productos
                .Include(p => p.IdDescuentos)
                .FirstOrDefaultAsync(
                    p => p.IdProducto == idProducto
                );

            // Comprueba que el producto exista.
            if (producto == null)
                return NotFound();

            // Busca el descuento dentro de las relaciones del producto.
            var descuento = producto.IdDescuentos
                .FirstOrDefault(
                    d => d.IdDescuento == idDescuento
                );

            // Comprueba que la relación exista.
            if (descuento == null)
                return NotFound();

            // Elimina la relación entre el producto y el descuento.
            producto.IdDescuentos.Remove(descuento);

            // Guarda los cambios en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve HTTP 204 indicando que se eliminó correctamente.
            return NoContent();
        }
    }
}