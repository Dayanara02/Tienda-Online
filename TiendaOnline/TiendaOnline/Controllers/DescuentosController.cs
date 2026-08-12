using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;
namespace TiendaOnline.API.Controllers

{

    // Define que esta clase pertenece al controlador de descuentos de la API.

    [Route("api/[controller]")]

    // Indica que esta clase es un controlador de una API.

    [ApiController]

    // Controlador encargado de gestionar las operaciones relacionadas con los descuentos.

    public class DescuentosController : ControllerBase

    {

        // Variable privada que permite acceder a la base de datos.

        private readonly TiendaOnlineContext _context;

        // Constructor del controlador.

        // Recibe el contexto de la base de datos mediante inyección de dependencias.

        public DescuentosController(TiendaOnlineContext context)

        {

            // Guarda el contexto recibido en la variable _context.

            _context = context;

        }

        // Método HTTP GET para obtener todos los descuentos.

        [HttpGet]

        public async Task<ActionResult<IEnumerable<Descuento>>> GetDescuentos()

        {

            // Consulta todos los descuentos registrados en la base de datos

            // y los convierte en una lista.

            return await _context.Descuentos.ToListAsync();

        }

        // Método HTTP GET que recibe el ID de un descuento.

        [HttpGet("{id}")]

        public async Task<ActionResult<Descuento>> GetDescuento(int id)

        {

            // Busca en la base de datos el descuento que tenga el ID indicado.

            var descuento = await _context.Descuentos.FindAsync(id);

            // Si no se encuentra el descuento, devuelve una respuesta NotFound (404).

            if (descuento == null)

                return NotFound();

            // Devuelve el descuento encontrado.

            return descuento;

        }

        // Indica que solamente los usuarios que tengan el rol de Administrador

        // pueden ejecutar este método.

        [Authorize(Roles = "Administrador")]

        // Método HTTP POST para crear un nuevo descuento.

        [HttpPost]

        public async Task<ActionResult<Descuento>> PostDescuento(Descuento descuento)

        {

            // Se establece el ID en 0 para que la base de datos genere

            // automáticamente el nuevo identificador.

            descuento.IdDescuento = 0;

            // Agrega el nuevo descuento al contexto de la base de datos.

            _context.Descuentos.Add(descuento);

            // Guarda los cambios realizados en la base de datos.

            await _context.SaveChangesAsync();

            // Devuelve una respuesta 201 Created indicando que el descuento

            // fue creado correctamente.

            return CreatedAtAction(

                nameof(GetDescuento),

                new { id = descuento.IdDescuento },

                descuento

            );

        }

        // Indica que solamente los usuarios que tengan el rol de Administrador

        // pueden ejecutar este método.

        [Authorize(Roles = "Administrador")]

        // Método HTTP PUT para actualizar un descuento existente.

        [HttpPut("{id}")]

        public async Task<IActionResult> PutDescuento(

            int id,

            Descuento descuento)

        {

            // Verifica que el ID recibido en la URL sea igual al ID

            // del descuento que se quiere modificar.

            if (id != descuento.IdDescuento)

                return BadRequest();

            // Busca el descuento existente en la base de datos.

            var existente = await _context.Descuentos.FindAsync(id);

            // Si no existe el descuento, devuelve una respuesta NotFound (404).

            if (existente == null)

                return NotFound();

            // Actualiza el nombre del descuento.

            existente.Nombre = descuento.Nombre;

            // Actualiza la descripción del descuento.

            existente.Descripcion = descuento.Descripcion;

            // Actualiza el porcentaje del descuento.

            existente.Porcentaje = descuento.Porcentaje;

            // Actualiza la fecha de inicio del descuento.

            existente.FechaInicio = descuento.FechaInicio;

            // Actualiza la fecha de finalización del descuento.

            existente.FechaFin = descuento.FechaFin;

            // Actualiza el estado del descuento.

            existente.Estado = descuento.Estado;

            // Guarda los cambios realizados en la base de datos.

            await _context.SaveChangesAsync();

            // Devuelve una respuesta 204 indicando que la actualización

            // se realizó correctamente y no hay contenido que devolver.

            return NoContent();

        }

        // Indica que solamente los usuarios que tengan el rol de Administrador

        // pueden ejecutar este método.

        [Authorize(Roles = "Administrador")]

        // Método HTTP DELETE para eliminar un descuento.

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteDescuento(int id)

        {

            // Busca en la base de datos el descuento que corresponde al ID recibido.

            var descuento = await _context.Descuentos.FindAsync(id);

            // Si no existe el descuento, devuelve una respuesta NotFound (404).

            if (descuento == null)

                return NotFound();

            // Elimina el descuento del contexto de la base de datos.

            _context.Descuentos.Remove(descuento);

            // Guarda los cambios para aplicar la eliminación en la base de datos.

            await _context.SaveChangesAsync();

            // Devuelve una respuesta 204 indicando que la eliminación

            // se realizó correctamente.

            return NoContent();

        }
    }
}