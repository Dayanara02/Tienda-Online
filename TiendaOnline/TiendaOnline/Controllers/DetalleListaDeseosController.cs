using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;


// Datos necesarios para agregar
// un producto a favoritos.
public class GuardarDeseoDto
{
    public int IdListaDeseos
    {
        get;
        set;
    }

    public int IdProducto
    {
        get;
        set;
    }
}


[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DetalleListaDeseosController
    : ControllerBase
{
    private readonly
        TiendaOnlineContext _context;


    public DetalleListaDeseosController(
        TiendaOnlineContext context)
    {
        _context =
            context;
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
            string? valor
            in posiblesIds
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
                    u =>
                        u.Correo ==
                        correo
                )
                .Select(
                    u =>
                        (int?)
                        u.IdUsuario
                )
                .FirstOrDefaultAsync();
        }


        return null;
    }


    // Obtiene detalles de una lista.
    [HttpGet(
        "lista/{idListaDeseos:int}"
    )]
    public async Task<IActionResult>
        GetDetallesLista(
            int idListaDeseos)
    {
        var idUsuario =
            await ObtenerIdUsuarioActual();


        if (!idUsuario.HasValue)
        {
            return Unauthorized();
        }


        var listaValida =
            await _context
                .ListaDeseos
                .AnyAsync(
                    l =>
                        l.IdListaDeseos ==
                            idListaDeseos
                        &&
                        l.IdUsuario ==
                            idUsuario.Value
                );


        if (!listaValida)
        {
            return NotFound();
        }


        var detalles =
            await _context
                .DetalleListaDeseos
                .AsNoTracking()
                .Where(
                    d =>
                        d.IdListaDeseos ==
                        idListaDeseos
                )
                .ToListAsync();


        return Ok(
            detalles
        );
    }


    // Agrega un favorito.
    [HttpPost]
    public async Task<IActionResult>
        PostDetalleListaDeseo(
            GuardarDeseoDto detalle)
    {
        var idUsuario =
            await ObtenerIdUsuarioActual();


        if (!idUsuario.HasValue)
        {
            return Unauthorized();
        }


        // Comprueba que la lista
        // sea del usuario.
        var listaValida =
            await _context
                .ListaDeseos
                .AnyAsync(
                    l =>
                        l.IdListaDeseos ==
                            detalle.IdListaDeseos
                        &&
                        l.IdUsuario ==
                            idUsuario.Value
                );


        if (!listaValida)
        {
            return BadRequest(
                "La lista no pertenece al usuario."
            );
        }


        // Verifica producto.
        var productoExiste =
            await _context
                .Productos
                .AnyAsync(
                    p =>
                        p.IdProducto ==
                        detalle.IdProducto
                );


        if (!productoExiste)
        {
            return NotFound(
                "El producto no existe."
            );
        }


        // Comprueba si ya existe.
        var existente =
            await _context
                .DetalleListaDeseos
                .FirstOrDefaultAsync(
                    d =>
                        d.IdListaDeseos ==
                            detalle.IdListaDeseos
                        &&
                        d.IdProducto ==
                            detalle.IdProducto
                );


        // Si ya existe simplemente
        // devuelve el mismo registro.
        if (existente != null)
        {
            return Ok(
                existente
            );
        }


        var nuevoDetalle =
            new DetalleListaDeseo
            {
                IdListaDeseos =
                    detalle.IdListaDeseos,

                IdProducto =
                    detalle.IdProducto,

                FechaAgregado =
                    DateTime.Now
            };


        _context.DetalleListaDeseos.Add(
            nuevoDetalle
        );


        await _context
            .SaveChangesAsync();


        return Ok(
            nuevoDetalle
        );
    }


    // Elimina un favorito.
    [HttpDelete(
        "{idListaDeseos:int}/{idProducto:int}"
    )]
    public async Task<IActionResult>
        DeleteDetalleListaDeseo(
            int idListaDeseos,
            int idProducto)
    {
        var idUsuario =
            await ObtenerIdUsuarioActual();


        if (!idUsuario.HasValue)
        {
            return Unauthorized();
        }


        var listaValida =
            await _context
                .ListaDeseos
                .AnyAsync(
                    l =>
                        l.IdListaDeseos ==
                            idListaDeseos
                        &&
                        l.IdUsuario ==
                            idUsuario.Value
                );


        if (!listaValida)
        {
            return NotFound();
        }


        var detalle =
            await _context
                .DetalleListaDeseos
                .FindAsync(
                    idListaDeseos,
                    idProducto
                );


        if (detalle == null)
        {
            return NotFound();
        }


        _context
            .DetalleListaDeseos
            .Remove(
                detalle
            );


        await _context
            .SaveChangesAsync();


        return NoContent();
    }
}