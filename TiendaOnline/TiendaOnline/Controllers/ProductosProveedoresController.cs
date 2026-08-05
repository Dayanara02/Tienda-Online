using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Model;
using Microsoft.AspNetCore.Authorization;

namespace TiendaOnline.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoProveedorsController : ControllerBase
    {
        private readonly TiendaOnlineContext _context;

        public ProductoProveedorsController(TiendaOnlineContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoProveedor>>> Get()
        {
            return await _context.ProductoProveedors.ToListAsync();
        }

        [HttpGet("{idProducto}/{idProveedor}")]
        public async Task<ActionResult<ProductoProveedor>> Get(
            int idProducto,
            int idProveedor)
        {
            var relacion = await _context.ProductoProveedors.FindAsync(
                idProducto,
                idProveedor
            );

            if (relacion == null)
                return NotFound();

            return relacion;
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<ActionResult<ProductoProveedor>> Post(
            ProductoProveedor relacion)
        {
            _context.ProductoProveedors.Add(relacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Get),
                new
                {
                    idProducto = relacion.IdProducto,
                    idProveedor = relacion.IdProveedor
                },
                relacion
            );
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{idProducto}/{idProveedor}")]
        public async Task<IActionResult> Put(
            int idProducto,
            int idProveedor,
            ProductoProveedor relacion)
        {
            if (idProducto != relacion.IdProducto ||
                idProveedor != relacion.IdProveedor)
                return BadRequest();

            var existente = await _context.ProductoProveedors.FindAsync(
                idProducto,
                idProveedor
            );

            if (existente == null)
                return NotFound();

            existente.PrecioCompra = relacion.PrecioCompra;
            existente.CodigoProveedor = relacion.CodigoProveedor;
            existente.Estado = relacion.Estado;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{idProducto}/{idProveedor}")]
        public async Task<IActionResult> Delete(
            int idProducto,
            int idProveedor)
        {
            var relacion = await _context.ProductoProveedors.FindAsync(
                idProducto,
                idProveedor
            );

            if (relacion == null)
                return NotFound();

            _context.ProductoProveedors.Remove(relacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
