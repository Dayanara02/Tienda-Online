// Importa las funcionalidades necesarias para crear controladores y respuestas HTTP.
using Microsoft.AspNetCore.Mvc;

// Importa Entity Framework Core para realizar consultas a la base de datos.
using Microsoft.EntityFrameworkCore;

// Importa el contexto de la base de datos de la tienda.
using TiendaOnline.AccesoDatos.Context;

// Importa las entidades utilizadas por el controlador.
using TiendaOnline.Dominio.Entidades;

// Importa las herramientas para controlar el acceso mediante roles.
using Microsoft.AspNetCore.Authorization;

// Define el espacio de nombres donde se encuentra el controlador.
namespace TiendaOnline.API.Controllers;

// Indica que solamente los usuarios con rol Administrador pueden acceder.
[Authorize(Roles = "Administrador")]

// Indica que esta clase funciona como un controlador de API.
[ApiController]

// Define la ruta principal para acceder al controlador.
[Route("api/[controller]")]
public class CompraProveedorsController : ControllerBase
{
    // Guarda el contexto utilizado para acceder a la base de datos.
    private readonly TiendaOnlineContext _context;

    // Constructor que recibe el contexto mediante inyección de dependencias.
    public CompraProveedorsController(
        TiendaOnlineContext context)
    {
        // Asigna el contexto recibido a la variable privada.
        _context = context;
    }

    // GET: api/CompraProveedors
    // Obtiene todas las compras realizadas a proveedores.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompraProveedor>>>
        GetCompraProveedors()
    {
        // Consulta las compras sin realizar seguimiento de cambios.
        return await _context.CompraProveedors
            .AsNoTracking()

            // Ejecuta la consulta y obtiene todos los registros.
            .ToListAsync();
    }

    // GET: api/CompraProveedors/5
    // Obtiene una compra específica utilizando su ID.
    [HttpGet("{id}")]
    public async Task<ActionResult<CompraProveedor>>
        GetCompraProveedor(int id)
    {
        // Busca la compra por su identificador.
        var compra =
            await _context.CompraProveedors.FindAsync(id);

        // Verifica si la compra no existe.
        if (compra == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Devuelve la compra encontrada.
        return compra;
    }

    // POST: api/CompraProveedors
    // Registra una nueva compra a un proveedor.
    [HttpPost]
    public async Task<ActionResult<CompraProveedor>>
        PostCompraProveedor(CompraProveedor compra)
    {
        // Establece el ID en cero para que sea generado por la base de datos.
        compra.IdCompraProveedor = 0;

        // Registra la fecha y hora en que se realiza la compra.
        compra.FechaCompra = DateTime.Now;

        // Verifica si no se indicó un estado para la compra.
        if (string.IsNullOrWhiteSpace(compra.Estado))
        {
            // Establece el estado inicial como Pendiente.
            compra.Estado = "Pendiente";
        }

        // Agrega la compra al contexto.
        _context.CompraProveedors.Add(compra);

        // Guarda la nueva compra en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta indicando que la compra fue creada.
        return CreatedAtAction(
            // Indica la acción utilizada para consultar la compra.
            nameof(GetCompraProveedor),

            // Envía el ID de la compra creada.
            new { id = compra.IdCompraProveedor },

            // Devuelve los datos de la compra registrada.
            compra
        );
    }

    // PUT: api/CompraProveedors/5
    // Actualiza los datos de una compra existente.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCompraProveedor(
        int id,
        CompraProveedor compra)
    {
        // Busca la compra existente mediante su identificador.
        var compraActual =
            await _context.CompraProveedors.FindAsync(id);

        // Verifica si la compra no existe.
        if (compraActual == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Actualiza el proveedor asociado a la compra.
        compraActual.IdProveedor = compra.IdProveedor;

        // Actualiza el usuario que realizó la compra.
        compraActual.IdUsuario = compra.IdUsuario;

        // Actualiza el subtotal de la compra.
        compraActual.Subtotal = compra.Subtotal;

        // Actualiza el impuesto aplicado a la compra.
        compraActual.Impuesto = compra.Impuesto;

        // Actualiza el total de la compra.
        compraActual.Total = compra.Total;

        // Actualiza el estado de la compra.
        compraActual.Estado = compra.Estado;

        // Guarda los cambios realizados en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se actualizó correctamente.
        return NoContent();
    }

    // DELETE: api/CompraProveedors/5
    // Elimina una compra registrada.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCompraProveedor(int id)
    {
        // Busca la compra mediante su identificador.
        var compra =
            await _context.CompraProveedors.FindAsync(id);

        // Verifica si la compra no existe.
        if (compra == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Marca la compra para ser eliminada.
        _context.CompraProveedors.Remove(compra);

        // Guarda la eliminación en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se eliminó correctamente.
        return NoContent();
    }
}