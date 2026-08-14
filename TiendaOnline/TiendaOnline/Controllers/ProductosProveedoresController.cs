// Permite crear controladores y manejar respuestas HTTP.
using Microsoft.AspNetCore.Mvc;

// Permite utilizar Entity Framework Core para trabajar con la base de datos.
using Microsoft.EntityFrameworkCore;

// Permite utilizar atributos de autorización como [Authorize].
using Microsoft.AspNetCore.Authorization;

// Importa el contexto de la base de datos.
using TiendaOnline.AccesoDatos.Context;

// Importa las entidades del proyecto.
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers
{
    // Define la ruta principal del controlador:
    // api/ProductoProveedors
    [Route("api/[controller]")]

    // Indica que la clase funciona como controlador de API.
    [ApiController]
    public class ProductoProveedorsController : ControllerBase
    {
        // Contexto utilizado para acceder a la base de datos.
        private readonly TiendaOnlineContext _context;

        // Constructor del controlador.
        public ProductoProveedorsController(
            TiendaOnlineContext context)
        {
            // Guarda el contexto recibido para utilizarlo
            // en las operaciones con la base de datos.
            _context = context;
        }


        // =========================================================
        // OBTENER TODAS LAS RELACIONES
        // =========================================================

        // GET: api/ProductoProveedors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoProveedor>>> Get()
        {
            // Obtiene todas las relaciones entre productos y proveedores.
            return await _context.ProductoProveedors
                .ToListAsync();
        }


        // =========================================================
        // OBTENER UNA RELACIÓN
        // =========================================================

        // GET: api/ProductoProveedors/1/2
        [HttpGet("{idProducto}/{idProveedor}")]
        public async Task<ActionResult<ProductoProveedor>> Get(
            int idProducto,
            int idProveedor)
        {
            // Busca la relación utilizando los dos campos
            // que forman la clave primaria.
            var relacion =
                await _context.ProductoProveedors.FindAsync(
                    idProducto,
                    idProveedor
                );

            // Comprueba si la relación existe.
            if (relacion == null)
                return NotFound();

            // Devuelve la relación encontrada.
            return relacion;
        }


        // =========================================================
        // CREAR UNA RELACIÓN
        // =========================================================

        // Solo los Administradores pueden crear relaciones.
        [Authorize(Roles = "Administrador")]

        // POST: api/ProductoProveedors
        [HttpPost]
        public async Task<ActionResult<ProductoProveedor>> Post(
            ProductoProveedor relacion)
        {
            // Agrega la relación entre el producto y el proveedor.
            _context.ProductoProveedors.Add(relacion);

            // Guarda los cambios en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve HTTP 201 indicando que se creó correctamente.
            return CreatedAtAction(
                nameof(Get),
                new
                {
                    // Identificador del producto.
                    idProducto = relacion.IdProducto,

                    // Identificador del proveedor.
                    idProveedor = relacion.IdProveedor
                },
                relacion
            );
        }


        // =========================================================
        // ACTUALIZAR UNA RELACIÓN
        // =========================================================

        // Solo los Administradores pueden modificar relaciones.
        [Authorize(Roles = "Administrador")]

        // PUT: api/ProductoProveedors/1/2
        [HttpPut("{idProducto}/{idProveedor}")]
        public async Task<IActionResult> Put(
            int idProducto,
            int idProveedor,
            ProductoProveedor relacion)
        {
            // Comprueba que los identificadores de la URL
            // coincidan con los enviados en el objeto.
            if (
                idProducto != relacion.IdProducto ||
                idProveedor != relacion.IdProveedor
            )
                return BadRequest();

            // Busca la relación existente.
            var existente =
                await _context.ProductoProveedors.FindAsync(
                    idProducto,
                    idProveedor
                );

            // Si no existe, devuelve HTTP 404.
            if (existente == null)
                return NotFound();

            // Actualiza el precio de compra.
            existente.PrecioCompra =
                relacion.PrecioCompra;

            // Actualiza el código asignado por el proveedor.
            existente.CodigoProveedor =
                relacion.CodigoProveedor;

            // Actualiza el estado de la relación.
            existente.Estado =
                relacion.Estado;

            // Guarda los cambios realizados.
            await _context.SaveChangesAsync();

            // Devuelve HTTP 204 indicando que se actualizó correctamente.
            return NoContent();
        }


        // =========================================================
        // ELIMINAR UNA RELACIÓN
        // =========================================================

        // Solo los Administradores pueden eliminar relaciones.
        [Authorize(Roles = "Administrador")]

        // DELETE: api/ProductoProveedors/1/2
        [HttpDelete("{idProducto}/{idProveedor}")]
        public async Task<IActionResult> Delete(
            int idProducto,
            int idProveedor)
        {
            // Busca la relación entre el producto y el proveedor.
            var relacion =
                await _context.ProductoProveedors.FindAsync(
                    idProducto,
                    idProveedor
                );

            // Comprueba si la relación existe.
            if (relacion == null)
                return NotFound();

            // Marca la relación para ser eliminada.
            _context.ProductoProveedors.Remove(relacion);

            // Guarda la eliminación en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve HTTP 204 indicando que se eliminó correctamente.
            return NoContent();
        }
    }
}