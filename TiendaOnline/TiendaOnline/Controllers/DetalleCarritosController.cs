using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;


// Datos necesarios para guardar
// un producto en el carrito.
public class GuardarDetalleCarritoDto
{
    public int IdCarrito
    {
        get;
        set;
    }

    public int IdProducto
    {
        get;
        set;
    }

    public int Cantidad
    {
        get;
        set;
    }

    public decimal PrecioUnitario
    {
        get;
        set;
    }
}


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DetalleCarritosController
    : ControllerBase
{
    private readonly
        TiendaOnlineContext _context;


    public DetalleCarritosController(
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


    // Obtiene todos los detalles.
    [HttpGet]
    public async Task<
        ActionResult<IEnumerable<DetalleCarrito>>>
        GetDetalleCarritos()
    {
        return await _context
            .DetalleCarritos
            .AsNoTracking()
            .ToListAsync();
    }


    // Guarda o actualiza un
    // producto del carrito.
    [HttpPost]
    public async Task<IActionResult>
        GuardarDetalleCarrito(
            GuardarDetalleCarritoDto detalle)
    {
        var idUsuario =
            await ObtenerIdUsuarioActual();


        if (!idUsuario.HasValue)
        {
            return Unauthorized();
        }


        // Verifica que el carrito
        // sea del usuario y esté activo.
        var carrito =
            await _context
                .Carritos
                .FirstOrDefaultAsync(
                    c =>
                        c.IdCarrito ==
                            detalle.IdCarrito
                        &&
                        c.IdUsuario ==
                            idUsuario.Value
                        &&
                        c.Estado ==
                            "Activo"
                );


        if (carrito == null)
        {
            return BadRequest(
                "El carrito no existe o no está activo."
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


        if (
            detalle.Cantidad <= 0
        )
        {
            return BadRequest(
                "La cantidad debe ser mayor a cero."
            );
        }


        // Busca si ya estaba agregado.
        var existente =
            await _context
                .DetalleCarritos
                .FirstOrDefaultAsync(
                    d =>
                        d.IdCarrito ==
                            detalle.IdCarrito
                        &&
                        d.IdProducto ==
                            detalle.IdProducto
                );


        // Si ya existe lo actualiza.
        if (existente != null)
        {
            existente.Cantidad =
                detalle.Cantidad;

            existente.PrecioUnitario =
                detalle.PrecioUnitario;


            await _context
                .SaveChangesAsync();


            return Ok(
                existente
            );
        }


        // Si no existe lo crea.
        var nuevoDetalle =
            new DetalleCarrito
            {
                IdCarrito =
                    detalle.IdCarrito,

                IdProducto =
                    detalle.IdProducto,

                Cantidad =
                    detalle.Cantidad,

                PrecioUnitario =
                    detalle.PrecioUnitario
            };


        _context.DetalleCarritos.Add(
            nuevoDetalle
        );


        await _context
            .SaveChangesAsync();


        return Ok(
            nuevoDetalle
        );
    }


    // Elimina un producto específico.
    [HttpDelete(
        "carrito/{idCarrito:int}/producto/{idProducto:int}"
    )]
    public async Task<IActionResult>
        EliminarProductoCarrito(
            int idCarrito,
            int idProducto)
    {
        var idUsuario =
            await ObtenerIdUsuarioActual();


        if (!idUsuario.HasValue)
        {
            return Unauthorized();
        }


        var carritoValido =
            await _context
                .Carritos
                .AnyAsync(
                    c =>
                        c.IdCarrito ==
                            idCarrito
                        &&
                        c.IdUsuario ==
                            idUsuario.Value
                );


        if (!carritoValido)
        {
            return NotFound();
        }


        var detalle =
            await _context
                .DetalleCarritos
                .FirstOrDefaultAsync(
                    d =>
                        d.IdCarrito ==
                            idCarrito
                        &&
                        d.IdProducto ==
                            idProducto
                );


        if (detalle == null)
        {
            return NotFound();
        }


        _context.DetalleCarritos.Remove(
            detalle
        );


        await _context
            .SaveChangesAsync();


        return NoContent();
    }


    // Vacía todos los productos.
    [HttpDelete(
        "carrito/{idCarrito:int}"
    )]
    public async Task<IActionResult>
        VaciarCarrito(
            int idCarrito)
    {
        var idUsuario =
            await ObtenerIdUsuarioActual();


        if (!idUsuario.HasValue)
        {
            return Unauthorized();
        }


        var carritoValido =
            await _context
                .Carritos
                .AnyAsync(
                    c =>
                        c.IdCarrito ==
                            idCarrito
                        &&
                        c.IdUsuario ==
                            idUsuario.Value
                );


        if (!carritoValido)
        {
            return NotFound();
        }


        var detalles =
            await _context
                .DetalleCarritos
                .Where(
                    d =>
                        d.IdCarrito ==
                        idCarrito
                )
                .ToListAsync();


        _context.DetalleCarritos
            .RemoveRange(
                detalles
            );


        await _context
            .SaveChangesAsync();


        return NoContent();
    }
}