using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Entidades;
using static QuestPDF.Helpers.Colors;
namespace TiendaOnline.API.Controllers;

// Indica que este controlador solo puede ser utilizado por usuarios // que tengan el rol de "Administrador".
[Authorize(Roles = "Administrador")]
[ApiController]
[Route("api/[controller]")]
public class DetalleCompraProveedorsController : ControllerBase
{   //Variable privada  que permite acceder a la base de datos
    //Merdiante el contexto TierndaOnlineContext
    private readonly TiendaOnlineContext _context;

    public DetalleCompraProveedorsController(
        TiendaOnlineContext context)
    {   //Guarda el contexto recibido en la variable_context 
        //Para utilizarlo en los diferentes metodos
        _context = context;
    }

    // GET: api/DetalleCompraProveedors
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DetalleCompraProveedor>>>
        GetDetalleCompraProveedors()
    {   //Consulta todos los registros de la tabla DetalleCompraProveedors.
        // AsNoTracking() indica que los registros no serán rastreados 
        // por Entity Framework, lo que mejora el rendimiento cuando
        // solamente se quieren consultar los datos.
        return await _context.DetalleCompraProveedors
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/DetalleCompraProveedors/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DetalleCompraProveedor>>
        GetDetalleCompraProveedor(int id)
    {
        var detalle =
            await _context.DetalleCompraProveedors.FindAsync(id);

        if (detalle == null)
        {
            return NotFound();
        }

        return detalle;
    }

    // POST: api/DetalleCompraProveedors
    [HttpPost]
    public async Task<ActionResult<DetalleCompraProveedor>>
        PostDetalleCompraProveedor(
            DetalleCompraProveedor detalle)
    {
        detalle.IdDetalleCompra = 0;

        _context.DetalleCompraProveedors.Add(detalle);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetDetalleCompraProveedor),
            new { id = detalle.IdDetalleCompra },
            detalle
        );
    }

    // PUT: api/DetalleCompraProveedors/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDetalleCompraProveedor(
        int id,
        DetalleCompraProveedor detalle)
    {
        var detalleActual =
            await _context.DetalleCompraProveedors.FindAsync(id);
        //Si el detalle no existe, devuelve una respuesta 404 Not Found
        if (detalleActual == null)
        {
            return NotFound();
        }

        detalleActual.IdCompraProveedor =
            detalle.IdCompraProveedor;
        detalleActual.IdProducto = detalle.IdProducto;
        detalleActual.Cantidad = detalle.Cantidad;
        detalleActual.PrecioUnitario =
            detalle.PrecioUnitario;
        detalleActual.Subtotal = detalle.Subtotal;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/DetalleCompraProveedors/5
    [HttpDelete("{id}")]
    public async Task<IActionResult>
        DeleteDetalleCompraProveedor(int id)
    {   //  Busca el detalle que se desea eliminar utilizando su ID
        var detalle =
            await _context.DetalleCompraProveedors.FindAsync(id);
        //Si no encuentra el detalle, devuelve una respuesta 404 Not
        if (detalle == null)
        {
            return NotFound();
        }

        _context.DetalleCompraProveedors.Remove(detalle);
        //Guarda los cambios y ejecuta la eliminacion en la base de datos 
        await _context.SaveChangesAsync();
        //Devuelve una respuesta 204 No Content,
        //Indicando que la eliminacion se realizo correctamente
        return NoContent();
    }
}
