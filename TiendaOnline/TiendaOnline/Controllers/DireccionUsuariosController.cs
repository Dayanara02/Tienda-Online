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

// Permite el acceso al controlador únicamente a usuarios autenticados.
[Authorize]

// Indica que esta clase funciona como un controlador de API.
[ApiController]

// Define la ruta principal para acceder al controlador.
[Route("api/[controller]")]
public class DireccionUsuariosController : ControllerBase
{
    // Guarda el contexto utilizado para acceder a la base de datos.
    private readonly TiendaOnlineContext _context;

    // Constructor que recibe el contexto mediante inyección de dependencias.
    public DireccionUsuariosController(
        TiendaOnlineContext context)
    {
        // Asigna el contexto recibido a la variable privada.
        _context = context;
    }

    // GET: api/DireccionUsuarios
    // Obtiene todas las direcciones registradas de los usuarios.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DireccionUsuario>>>
        GetDireccionUsuarios()
    {
        // Consulta las direcciones sin realizar seguimiento de cambios.
        return await _context.DireccionUsuarios
            .AsNoTracking()

            // Ejecuta la consulta y obtiene todos los registros.
            .ToListAsync();
    }

    // GET: api/DireccionUsuarios/5
    // Obtiene una dirección específica mediante su ID.
    [HttpGet("{id}")]
    public async Task<ActionResult<DireccionUsuario>>
        GetDireccionUsuario(int id)
    {
        // Busca la dirección utilizando su identificador.
        var direccion =
            await _context.DireccionUsuarios.FindAsync(id);

        // Verifica si la dirección no existe.
        if (direccion == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Devuelve la dirección encontrada.
        return direccion;
    }

    // POST: api/DireccionUsuarios
    // Registra una nueva dirección para un usuario.
    [HttpPost]
    public async Task<ActionResult<DireccionUsuario>>
        PostDireccionUsuario(
            DireccionUsuario direccion)
    {
        // Establece el ID en cero para que la base de datos lo genere.
        direccion.IdDireccion = 0;

        // Verifica si la nueva dirección será la principal.
        if (direccion.Principal)
        {
            // Busca las direcciones principales del mismo usuario.
            var direccionesPrincipales =
                await _context.DireccionUsuarios
                    .Where(d =>
                        d.IdUsuario == direccion.IdUsuario &&
                        d.Principal)
                    .ToListAsync();

            // Recorre las direcciones principales encontradas.
            foreach (var actual in direccionesPrincipales)
            {
                // Cambia las direcciones anteriores para que no sean principales.
                actual.Principal = false;
            }
        }

        // Agrega la nueva dirección al contexto.
        _context.DireccionUsuarios.Add(direccion);

        // Guarda la nueva dirección en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta indicando que la dirección fue creada.
        return CreatedAtAction(
            // Indica la acción utilizada para consultar la dirección.
            nameof(GetDireccionUsuario),

            // Envía el ID de la dirección creada.
            new { id = direccion.IdDireccion },

            // Devuelve los datos de la dirección registrada.
            direccion
        );
    }

    // PUT: api/DireccionUsuarios/5
    // Actualiza una dirección existente.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDireccionUsuario(
        int id,
        DireccionUsuario direccion)
    {
        // Busca la dirección existente mediante su identificador.
        var direccionActual =
            await _context.DireccionUsuarios.FindAsync(id);

        // Verifica si la dirección no existe.
        if (direccionActual == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Verifica si la dirección actualizada será la principal.
        if (direccion.Principal)
        {
            // Busca otras direcciones principales del mismo usuario.
            var otrasDirecciones =
                await _context.DireccionUsuarios
                    .Where(d =>
                        d.IdUsuario == direccion.IdUsuario &&
                        d.IdDireccion != id &&
                        d.Principal)
                    .ToListAsync();

            // Recorre las otras direcciones principales encontradas.
            foreach (var actual in otrasDirecciones)
            {
                // Cambia las otras direcciones para que no sean principales.
                actual.Principal = false;
            }
        }

        // Actualiza el usuario asociado a la dirección.
        direccionActual.IdUsuario = direccion.IdUsuario;

        // Actualiza la provincia de la dirección.
        direccionActual.Provincia = direccion.Provincia;

        // Actualiza el cantón de la dirección.
        direccionActual.Canton = direccion.Canton;

        // Actualiza el distrito de la dirección.
        direccionActual.Distrito = direccion.Distrito;

        // Actualiza la dirección exacta.
        direccionActual.DireccionExacta =
            direccion.DireccionExacta;

        // Actualiza el código postal.
        direccionActual.CodigoPostal =
            direccion.CodigoPostal;

        // Actualiza si la dirección es principal.
        direccionActual.Principal = direccion.Principal;

        // Actualiza el estado de la dirección.
        direccionActual.Estado = direccion.Estado;

        // Guarda los cambios realizados en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se actualizó correctamente.
        return NoContent();
    }

    // DELETE: api/DireccionUsuarios/5
    // Elimina una dirección existente.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDireccionUsuario(
        int id)
    {
        // Busca la dirección mediante su identificador.
        var direccion =
            await _context.DireccionUsuarios.FindAsync(id);

        // Verifica si la dirección no existe.
        if (direccion == null)
        {
            // Devuelve una respuesta 404.
            return NotFound();
        }

        // Marca la dirección para ser eliminada.
        _context.DireccionUsuarios.Remove(direccion);

        // Guarda la eliminación en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve una respuesta 204 indicando que se eliminó correctamente.
        return NoContent();
    }
}