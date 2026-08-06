using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Model;
using Microsoft.AspNetCore.Authorization;

namespace TiendaOnline.API.Controllers;

[Authorize(Roles = "Administrador")]
[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public RolesController(TiendaOnlineContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Rol>>> GetRoles()
    {
        return await _context.Rols.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Rol>> GetRol(int id)
    {
        var rol = await _context.Rols.FindAsync(id);

        if (rol == null)
            return NotFound();

        return rol;
    }

    [HttpPost]
    public async Task<ActionResult<Rol>> PostRol(Rol rol)
    {
        _context.Rols.Add(rol);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRol), new { id = rol.IdRol }, rol);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutRol(int id, Rol rol)
    {
        if (id != rol.IdRol)
            return BadRequest();

        _context.Entry(rol).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRol(int id)
    {
        var rol = await _context.Rols.FindAsync(id);

        if (rol == null)
            return NotFound();

        _context.Rols.Remove(rol);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}