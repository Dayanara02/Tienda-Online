using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarritosController : ControllerBase
{
    private readonly TiendaOnlineContext _context;

    public CarritosController(
        TiendaOnlineContext context)
    {
        _context = context;
    }


    // Obtiene el usuario conectado.
    private async Task<int?>
        ObtenerIdUsuarioActual()
    {
        string?[] posiblesIds =
        {
            User.FindFirst(
                ClaimTypes.NameIdentifier
            )?.Value,

            User.FindFirst(
                "idUsuario"
            )?.Value,

            User.FindFirst(
                "IdUsuario"
            )?.Value,

            User.FindFirst(
                "sub"
            )?.Value,

            User.FindFirst(
                "nameid"
            )?.Value
        };


        foreach (
            string? valor in posiblesIds
        )
        {
            if (
                int.TryParse(
                    valor,
                    out int idUsuario
                )
                &&
                idUsuario > 0
            )
            {
                return idUsuario;
            }
        }


        var correo =
            User.FindFirst(
                ClaimTypes.Email
            )?.Value
            ??
            User.FindFirst(
                "email"
            )?.Value;


        if (
            !string.IsNullOrWhiteSpace(
                correo
            )
        )
        {
            return await _context
                .Usuarios
                .Where(
                    usuario =>
                        usuario.Correo ==
                        correo
                )
                .Select(
                    usuario =>
                        (int?)
                        usuario.IdUsuario
                )
                .FirstOrDefaultAsync();
        }


        return null;
    }


    // Obtiene todos los carritos.
    [HttpGet]
    public async Task<
        ActionResult<IEnumerable<Carrito>>>
        GetCarritos()
    {
        return await _context
            .Carritos
            .AsNoTracking()
            .ToListAsync();
    }


    // Obtiene un carrito.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Carrito>>
        GetCarrito(
            int id)
    {
        var carrito =
            await _context
                .Carritos
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    c =>
                        c.IdCarrito ==
                        id
                );


        if (carrito == null)
        {
            return NotFound();
        }


        return Ok(
            carrito
        );
    }


    // Obtiene o crea el carrito
    // activo del usuario conectado.
    [HttpPost("actual")]
    public async Task<ActionResult<Carrito>>
        ObtenerOCrearCarritoActual()
    {
        var idUsuario =
            await ObtenerIdUsuarioActual();


        if (!idUsuario.HasValue)
        {
            return Unauthorized(
                "No se pudo identificar al usuario."
            );
        }


        // Busca todos los carritos
        // activos del usuario.
        var carritosActivos =
            await _context
                .Carritos
                .Where(
                    c =>
                        c.IdUsuario ==
                        idUsuario.Value
                        &&
                        c.Estado ==
                        "Activo"
                )
                .OrderByDescending(
                    c =>
                        c.FechaCreacion
                )
                .ThenByDescending(
                    c =>
                        c.IdCarrito
                )
                .ToListAsync();


        // Si existe alguno...
        if (
            carritosActivos.Count > 0
        )
        {
            // Conserva solamente el más nuevo.
            var carritoActual =
                carritosActivos[0];


            // Si por algún error había
            // más de uno activo...
            foreach (
                var carritoAnterior
                in carritosActivos.Skip(1)
            )
            {
                carritoAnterior.Estado =
                    "Inactivo";
            }


            if (
                carritosActivos.Count > 1
            )
            {
                await _context
                    .SaveChangesAsync();
            }


            return Ok(
                carritoActual
            );
        }


        // Crea un carrito nuevo.
        var nuevoCarrito =
            new Carrito
            {
                IdUsuario =
                    idUsuario.Value,

                FechaCreacion =
                    DateTime.Now,

                Estado =
                    "Activo"
            };


        _context.Carritos.Add(
            nuevoCarrito
        );


        await _context
            .SaveChangesAsync();


        return Ok(
            nuevoCarrito
        );
    }


    // Marca el carrito actual
    // como Inactivo.
    [HttpPut("actual/inactivar")]
    public async Task<IActionResult>
        InactivarCarritoActual()
    {
        var idUsuario =
            await ObtenerIdUsuarioActual();


        if (!idUsuario.HasValue)
        {
            return Unauthorized();
        }


        var carrito =
            await _context
                .Carritos
                .Where(
                    c =>
                        c.IdUsuario ==
                        idUsuario.Value
                        &&
                        c.Estado ==
                        "Activo"
                )
                .OrderByDescending(
                    c =>
                        c.FechaCreacion
                )
                .FirstOrDefaultAsync();


        if (carrito == null)
        {
            return NotFound(
                "No existe un carrito activo."
            );
        }


        carrito.Estado =
            "Inactivo";


        await _context
            .SaveChangesAsync();


        return Ok(
            new
            {
                mensaje =
                    "Carrito marcado como Inactivo.",

                idCarrito =
                    carrito.IdCarrito,

                estado =
                    carrito.Estado
            }
        );
    }


    // Crea un carrito manualmente.
    [HttpPost]
    public async Task<ActionResult<Carrito>>
        PostCarrito(
            Carrito carrito)
    {
        carrito.IdCarrito =
            0;

        carrito.FechaCreacion =
            DateTime.Now;


        if (
            string.IsNullOrWhiteSpace(
                carrito.Estado
            )
        )
        {
            carrito.Estado =
                "Activo";
        }


        _context.Carritos.Add(
            carrito
        );


        await _context
            .SaveChangesAsync();


        return CreatedAtAction(
            nameof(GetCarrito),

            new
            {
                id =
                    carrito.IdCarrito
            },

            carrito
        );
    }


    // Actualiza un carrito.
    [HttpPut("{id:int}")]
    public async Task<IActionResult>
        PutCarrito(
            int id,
            Carrito carrito)
    {
        var actual =
            await _context
                .Carritos
                .FindAsync(id);


        if (actual == null)
        {
            return NotFound();
        }


        actual.Estado =
            carrito.Estado;


        await _context
            .SaveChangesAsync();


        return NoContent();
    }


    // Elimina un carrito.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
        DeleteCarrito(
            int id)
    {
        var carrito =
            await _context
                .Carritos
                .FindAsync(id);


        if (carrito == null)
        {
            return NotFound();
        }


        _context.Carritos.Remove(
            carrito
        );


        await _context
            .SaveChangesAsync();


        return NoContent();
    }
}