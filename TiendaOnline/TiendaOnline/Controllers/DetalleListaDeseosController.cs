using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers;
//Permite el acceso al controlador unicamente a usuarios autenticados 
[Authorize]
[ApiController]
[Route("api/[controller]")]

//Define la ruta base del controlador
public class DetalleListaDeseosController : ControllerBase
{
    private readonly TiendaOnlineContext _context;
    //Constructor del controlador 
    //Recibe el contexto de la base de datos mediante inyeccion de dependencias
    public DetalleListaDeseosController(
        TiendaOnlineContext context)
    {
        _context = context;
    }

    // GET: api/DetalleListaDeseos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DetalleListaDeseo>>>
        GetDetalleListaDeseos()
    {
        return await _context.DetalleListaDeseos
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/DetalleListaDeseos/1/5
    [HttpGet("{idListaDeseos}/{idProducto}")]
    public async Task<ActionResult<DetalleListaDeseo>>
        GetDetalleListaDeseo(
            int idListaDeseos,
            int idProducto)
    {
        var detalle = await _context.DetalleListaDeseos
            .FindAsync(idListaDeseos, idProducto);

        if (detalle == null)
        {
            return NotFound();
        }

        return detalle;
    }

    // POST: api/DetalleListaDeseos
    [HttpPost]
    public async Task<ActionResult<DetalleListaDeseo>>
        PostDetalleListaDeseo(
            DetalleListaDeseo detalle)
    {
        detalle.FechaAgregado = DateTime.Now;

        var existe = await _context.DetalleListaDeseos
            .AnyAsync(d =>
                d.IdListaDeseos == detalle.IdListaDeseos &&
                d.IdProducto == detalle.IdProducto);
        //Si el producto ya existe, devuelve un error 409 Conflict
        //Indicando que no se puede agregar nuevamente 
        if (existe)
        {
            return Conflict(
                "El producto ya está agregado a la lista de deseos."
            );
        }

        _context.DetalleListaDeseos.Add(detalle);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetDetalleListaDeseo),
            new
            {
                idListaDeseos = detalle.IdListaDeseos,
                idProducto = detalle.IdProducto
            },
            detalle
        );
    }

    // PUT: api/DetalleListaDeseos/1/5
    //Permite actualizar un detalle existente de la lista de deseos 
    [HttpPut("{idListaDeseos}/{idProducto}")]
    public async Task<IActionResult> PutDetalleListaDeseo(
        int idListaDeseos,
        int idProducto,
        DetalleListaDeseo detalle)
    {
        var detalleActual = await _context.DetalleListaDeseos
            .FindAsync(idListaDeseos, idProducto);

        if (detalleActual == null)
        {
            return NotFound();
        }

        detalleActual.FechaAgregado = detalle.FechaAgregado;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/DetalleListaDeseos/1/5
    [HttpDelete("{idListaDeseos}/{idProducto}")]
    public async Task<IActionResult> DeleteDetalleListaDeseo(
        int idListaDeseos,
        int idProducto)
    {   //Busca el detalle utilizando el ID de la lista de deseos 
        var detalle = await _context.DetalleListaDeseos
            .FindAsync(idListaDeseos, idProducto);

        if (detalle == null)
        {
            return NotFound();
        }

        _context.DetalleListaDeseos.Remove(detalle);
        await _context.SaveChangesAsync();
        //Indica que la eliminacion se realizo correctamente 
        return NoContent();
    }
}