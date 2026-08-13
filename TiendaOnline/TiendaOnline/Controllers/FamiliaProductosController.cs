using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;

// Define la ruta principal del controlador.
// [controller] será reemplazado por "FamiliaProductos"
[Route("api/[controller]")]
[ApiController]

// Indica que esta clase funciona como un controlador de una API.
public class FamiliaProductosController : ControllerBase
{
    private readonly TiendaOnlineContext _context;
    // Constructor del controlador.
    // Recibe el contexto de la base de datos mediante
    // inyección de dependencias.
    public FamiliaProductosController(TiendaOnlineContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FamiliaProducto>>> GetFamilias()

    {    // Consulta todos los registros de la tabla FamiliaProductos
        // y los devuelve como una lista.
        return await _context.FamiliaProductos.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FamiliaProducto>> GetFamilia(int id)
    {
        var familia = await _context.FamiliaProductos.FindAsync(id);

        if (familia == null)
            return NotFound();

        return familia;
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<ActionResult<FamiliaProducto>> PostFamilia(
        FamiliaProducto familia)
    {
        familia.IdFamilia = 0;

        _context.FamiliaProductos.Add(familia);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetFamilia),
            new { id = familia.IdFamilia },
            familia
        );
    }

    [Authorize(Roles = "Administrador")]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutFamilia(
        int id,
        FamiliaProducto familia)
    {
        if (id != familia.IdFamilia)
            return BadRequest();
        // Busca en la base de datos una familia utilizando
        // el identificador recibido+ 
        var existente = await _context.FamiliaProductos.FindAsync(id);

        if (existente == null)
            return NotFound();

        existente.Nombre = familia.Nombre;
        existente.Descripcion = familia.Descripcion;
        existente.Estado = familia.Estado;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFamilia(int id)
    {
        var familia = await _context.FamiliaProductos.FindAsync(id);

        if (familia == null)
            return NotFound();

        _context.FamiliaProductos.Remove(familia);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}