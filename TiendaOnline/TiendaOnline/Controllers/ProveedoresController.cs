using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly TiendaOnlineContext _context;

        public ProveedoresController(TiendaOnlineContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetProveedores()
        {
            return await _context.Proveedors.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Proveedor>> GetProveedor(int id)
        {
            var proveedor = await _context.Proveedors.FindAsync(id);

            if (proveedor == null)
                return NotFound();

            return proveedor;
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<ActionResult<Proveedor>> PostProveedor(
            Proveedor proveedor)
        {
            proveedor.IdProveedor = 0;

            _context.Proveedors.Add(proveedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProveedor),
                new { id = proveedor.IdProveedor },
                proveedor
            );
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProveedor(
            int id,
            Proveedor proveedor)
        {
            if (id != proveedor.IdProveedor)
                return BadRequest();

            var existente = await _context.Proveedors.FindAsync(id);

            if (existente == null)
                return NotFound();

            existente.Nombre = proveedor.Nombre;
            existente.Identificacion = proveedor.Identificacion;
            existente.Correo = proveedor.Correo;
            existente.Telefono = proveedor.Telefono;
            existente.Direccion = proveedor.Direccion;
            existente.Estado = proveedor.Estado;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            var proveedor = await _context.Proveedors.FindAsync(id);

            if (proveedor == null)
                return NotFound();

            _context.Proveedors.Remove(proveedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}