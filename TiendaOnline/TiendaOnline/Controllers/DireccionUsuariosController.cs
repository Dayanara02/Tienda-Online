using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;
// que el usuario esté autenticado para poder acceder
[Authorize]
// Indica que esta clase funciona como un controlador de una API
[ApiController]
[Route("api/[controller]")]
public class DireccionUsuariosController : ControllerBase
{   // Variable privada que contiene el contexto de la base de datos.
    // Permite realizar consultas y modificaciones mediante Entity Framework.
    private readonly TiendaOnlineContext _context;

    public DireccionUsuariosController(
        TiendaOnlineContext context)
    {  // Se asigna el contexto recibido a la variable _context.
        _context = context;
    }

    // GET: api/DireccionUsuarios
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DireccionUsuario>>>
        GetDireccionUsuarios()
    {
        // Consulta la tabla DireccionUsuarios.
        // AsNoTracking() indica que los registros solamente se
        // están consultando y no serán modificados.
        // ToListAsync() obtiene todos los registros de forma asíncrona.
        return await _context.DireccionUsuarios
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/DireccionUsuarios/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DireccionUsuario>>
        GetDireccionUsuario(int id)
    {
        var direccion =
            await _context.DireccionUsuarios.FindAsync(id);
        // Si no existe una dirección con ese ID,
        // se devuelve una respuesta HTTP 404 (Not Found).
        if (direccion == null)
        {
            return NotFound();
        }

        return direccion;
    }

    // POST: api/DireccionUsuarios
    [HttpPost]
    public async Task<ActionResult<DireccionUsuario>>
        PostDireccionUsuario(
            DireccionUsuario direccion)
    {
        direccion.IdDireccion = 0;

        if (direccion.Principal)
        {
            var direccionesPrincipales =
                await _context.DireccionUsuarios
                    .Where(d =>
                        d.IdUsuario == direccion.IdUsuario &&
                        d.Principal)
                    .ToListAsync();

            foreach (var actual in direccionesPrincipales)
            {
                actual.Principal = false;
            }
        }

        _context.DireccionUsuarios.Add(direccion);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetDireccionUsuario),
            new { id = direccion.IdDireccion },
            direccion
        );
    }

    // PUT: api/DireccionUsuarios/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDireccionUsuario(
        int id,
        DireccionUsuario direccion)
    {
        var direccionActual =
            await _context.DireccionUsuarios.FindAsync(id);

        if (direccionActual == null)
        {
            return NotFound();
        }

        if (direccion.Principal)
        {
            var otrasDirecciones =
                await _context.DireccionUsuarios
                    .Where(d =>
                        d.IdUsuario == direccion.IdUsuario &&
                        d.IdDireccion != id &&
                        d.Principal)
                    .ToListAsync();

            foreach (var actual in otrasDirecciones)
            {
                actual.Principal = false;
            }
        }

        direccionActual.IdUsuario = direccion.IdUsuario;
        direccionActual.Provincia = direccion.Provincia;
        direccionActual.Canton = direccion.Canton;
        direccionActual.Distrito = direccion.Distrito;
        direccionActual.DireccionExacta =
            direccion.DireccionExacta;
        direccionActual.CodigoPostal =
            direccion.CodigoPostal;
        direccionActual.Principal = direccion.Principal;
        direccionActual.Estado = direccion.Estado;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/DireccionUsuarios/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDireccionUsuario(
        int id)
    {
        var direccion =
            await _context.DireccionUsuarios.FindAsync(id);

        if (direccion == null)
        {
            return NotFound();
        }

        _context.DireccionUsuarios.Remove(direccion);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}