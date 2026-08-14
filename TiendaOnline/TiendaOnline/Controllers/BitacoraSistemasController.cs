using Microsoft.AspNetCore.Mvc; // Permite crear controladores y responder a solicitudes HTTP.
using Microsoft.EntityFrameworkCore; // Permite realizar consultas a la base de datos.
using TiendaOnline.AccesoDatos.Context; // Contiene el contexto de la base de datos.
using Microsoft.AspNetCore.Authorization; // Permite controlar el acceso a los métodos.
using TiendaOnline.Dominio.Entidades; // Contiene las entidades utilizadas.

namespace TiendaOnline.API.Controllers; // Define el espacio donde está el controlador.

[Authorize(Roles = "Administrador")] // Permite acceder solo a usuarios administradores.
[ApiController] // Indica que esta clase funciona como controlador de una API.
[Route("api/[controller]")] // Define la ruta principal del controlador.
public class BitacoraSistemasController : ControllerBase
{
    private readonly TiendaOnlineContext _context; // Guarda la conexión con la base de datos.

    public BitacoraSistemasController(
        TiendaOnlineContext context)
    {
        _context = context; // Recibe y guarda el contexto para usarlo en los métodos.
    }

    // GET: api/BitacoraSistemas
    [HttpGet] // Indica que este método responde a una solicitud GET.
    public async Task<ActionResult<IEnumerable<BitacoraSistema>>>
        GetBitacoraSistemas()
    {
        return await _context.BitacoraSistemas
            .AsNoTracking() // Indica que los datos solo se van a consultar.
            .OrderByDescending(b => b.Fecha) // Ordena los registros del más reciente al más antiguo.
            .ToListAsync(); // Ejecuta la consulta y obtiene todos los registros.
    }

    // GET: api/BitacoraSistemas/5
    [HttpGet("{id}")] // Permite buscar un registro utilizando su ID.
    public async Task<ActionResult<BitacoraSistema>>
        GetBitacoraSistema(int id)
    {
        var bitacora =
            await _context.BitacoraSistemas.FindAsync(id); // Busca la bitácora por su ID.

        if (bitacora == null) // Comprueba si no se encontró ningún registro.
        {
            return NotFound(); // Devuelve una respuesta indicando que no existe.
        }

        return bitacora; // Devuelve la bitácora encontrada.
    }

    // POST: api/BitacoraSistemas
    [HttpPost] // Indica que este método permite registrar una nueva bitácora.
    public async Task<ActionResult<BitacoraSistema>>
        PostBitacoraSistema(
            BitacoraSistema bitacora)
    {
        bitacora.IdBitacora = 0; // Deja el ID en cero para que la base de datos lo genere.
        bitacora.Fecha = DateTime.Now; // Asigna la fecha y hora actual al registro.

        _context.BitacoraSistemas.Add(bitacora); // Agrega la bitácora al contexto.
        await _context.SaveChangesAsync(); // Guarda el nuevo registro en la base de datos.

        return CreatedAtAction(
            nameof(GetBitacoraSistema), // Indica el método para consultar el registro creado.
            new { id = bitacora.IdBitacora }, // Envía el ID del registro creado.
            bitacora // Devuelve los datos de la bitácora registrada.
        );
    }
}