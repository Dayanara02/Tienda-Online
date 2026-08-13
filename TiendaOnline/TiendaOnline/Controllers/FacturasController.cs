// Importa las funcionalidades necesarias para crear controladores y respuestas HTTP.
using Microsoft.AspNetCore.Mvc;

// Importa Entity Framework Core para realizar consultas a la base de datos.
using Microsoft.EntityFrameworkCore;

// Importa el contexto de la base de datos de la tienda.
using TiendaOnline.AccesoDatos.Context;

// Importa las herramientas para controlar el acceso de los usuarios.
using Microsoft.AspNetCore.Authorization;

// Importa las entidades utilizadas por el controlador.
using TiendaOnline.Dominio.Entidades;

// Define el espacio de nombres donde se encuentra el controlador.
namespace TiendaOnline.API.Controllers;

// Permite el acceso únicamente a usuarios autenticados.
[Authorize]

// Indica que esta clase funciona como un controlador de API.
[ApiController]

// Define la ruta principal para acceder al controlador.
[Route("api/[controller]")]
public class FacturasController : ControllerBase
{
    // Guarda el contexto utilizado para acceder a la base de datos.
    private readonly TiendaOnlineContext _context;

    // Constructor que recibe el contexto mediante inyección de dependencias.
    public FacturasController(TiendaOnlineContext context)
    {
        // Asigna el contexto recibido a la variable privada.
        _context = context;
    }

    // GET: api/Facturas
    // Obtiene todas las facturas registradas.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Factura>>> GetFacturas()
    {
        // Consulta las facturas sin realizar seguimiento de cambios.
        return await _context.Facturas
            .AsNoTracking()

            // Ejecuta la consulta y obtiene todos los registros.
            .ToListAsync();
    }

    // GET: api/Facturas/5
    // Obtiene una factura específica mediante su ID.
    [HttpGet("{id}")]
    public async Task<ActionResult<Factura>> GetFactura(int id)
    {
        // Busca la factura utilizando su identificador.
        var factura = await _context.Facturas.FindAsync(id);

        // Verifica si la factura no existe.
        if (factura == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Devuelve la factura encontrada.
        return factura;
    }

    // POST: api/Facturas
    // Registra una nueva factura.
    [HttpPost]
    public async Task<ActionResult<Factura>> PostFactura(
        Factura factura)
    {
        // Establece el ID en cero para que la base de datos lo genere.
        factura.IdFactura = 0;

        // Registra automáticamente la fecha y hora de emisión.
        factura.FechaEmision = DateTime.Now;

        // Agrega la factura al contexto de la base de datos.
        _context.Facturas.Add(factura);

        // Guarda la nueva factura en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta indicando que la factura fue creada.
        return CreatedAtAction(
            // Indica la acción utilizada para consultar la factura.
            nameof(GetFactura),

            // Envía el ID de la factura creada.
            new { id = factura.IdFactura },

            // Devuelve los datos de la factura registrada.
            factura
        );
    }

    // PUT: api/Facturas/5
    // Actualiza una factura existente.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutFactura(
        int id,
        Factura factura)
    {
        // Busca la factura existente mediante su identificador.
        var facturaActual = await _context.Facturas.FindAsync(id);

        // Verifica si la factura no existe.
        if (facturaActual == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Actualiza el pedido asociado a la factura.
        facturaActual.IdPedido = factura.IdPedido;

        // Actualiza el número de factura.
        facturaActual.NumeroFactura = factura.NumeroFactura;

        // Actualiza el subtotal de la factura.
        facturaActual.Subtotal = factura.Subtotal;

        // Actualiza el impuesto aplicado.
        facturaActual.Impuesto = factura.Impuesto;

        // Actualiza el descuento aplicado.
        facturaActual.Descuento = factura.Descuento;

        // Actualiza el total de la factura.
        facturaActual.Total = factura.Total;

        // Actualiza la URL del archivo PDF de la factura.
        facturaActual.UrlPdf = factura.UrlPdf;

        // Guarda los cambios realizados en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se actualizó correctamente.
        return NoContent();
    }

    // DELETE: api/Facturas/5
    // Elimina una factura existente.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFactura(int id)
    {
        // Busca la factura mediante su identificador.
        var factura = await _context.Facturas.FindAsync(id);

        // Verifica si la factura no existe.
        if (factura == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Marca la factura para ser eliminada.
        _context.Facturas.Remove(factura);

        // Guarda la eliminación en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se eliminó correctamente.
        return NoContent();
    }
}