using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ListaDeseosController
    : ControllerBase
{
    private readonly
        TiendaOnlineContext _context;


    public ListaDeseosController(
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


    // Obtiene todas las listas.
    [HttpGet]
    public async Task<
        ActionResult<IEnumerable<ListaDeseo>>>
        GetListaDeseos()
    {
        return await _context
            .ListaDeseos
            .AsNoTracking()
            .ToListAsync();
    }


    // Obtiene o crea la lista
    // del usuario conectado.
    [HttpPost("actual")]
    public async Task<IActionResult>
        ObtenerOCrearListaActual()
    {
        var idUsuario =
            await ObtenerIdUsuarioActual();


        if (!idUsuario.HasValue)
        {
            return Unauthorized(
                "No se pudo identificar al usuario."
            );
        }


        var lista =
            await _context
                .ListaDeseos
                .FirstOrDefaultAsync(
                    l =>
                        l.IdUsuario ==
                        idUsuario.Value
                );


        if (lista != null)
        {
            return Ok(
                lista
            );
        }


        lista =
            new ListaDeseo
            {
                IdUsuario =
                    idUsuario.Value,

                FechaCreacion =
                    DateTime.Now
            };


        _context.ListaDeseos.Add(
            lista
        );


        await _context
            .SaveChangesAsync();


        return Ok(
            lista
        );
    }


    // Obtiene una lista.
    [HttpGet("{id:int}")]
    public async Task<
        ActionResult<ListaDeseo>>
        GetListaDeseo(
            int id)
    {
        var lista =
            await _context
                .ListaDeseos
                .FindAsync(id);


        if (lista == null)
        {
            return NotFound();
        }


        return Ok(
            lista
        );
    }
}