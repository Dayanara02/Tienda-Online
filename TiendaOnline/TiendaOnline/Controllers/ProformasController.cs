// Permite crear controladores y manejar respuestas HTTP.
using Microsoft.AspNetCore.Mvc;

// Permite utilizar Entity Framework Core para trabajar con la base de datos.
using Microsoft.EntityFrameworkCore;

// Permite utilizar atributos de autorización como [Authorize].
using Microsoft.AspNetCore.Authorization;

// Importa el contexto de la base de datos.
using TiendaOnline.AccesoDatos.Context;

// Importa las entidades del proyecto.
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers
{
    // Requiere que el usuario esté autenticado para acceder al controlador.
    [Authorize]

    // Indica que esta clase funciona como controlador de API.
    [ApiController]

    // Define la ruta principal:
    // api/Proformas
    [Route("api/[controller]")]
    public class ProformasController : ControllerBase
    {
        // Contexto utilizado para consultar y modificar
        // información en la base de datos.
        private readonly TiendaOnlineContext _context;

        // Constructor del controlador.
        public ProformasController(
            TiendaOnlineContext context)
        {
            // Guarda el contexto recibido.
            _context = context;
        }


        // =========================================================
        // OBTENER TODAS LAS PROFORMAS
        // =========================================================

        // GET: api/Proformas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proforma>>>
            GetProformas()
        {
            // Obtiene todas las proformas.
            return await _context.Proformas

                // Como solo se consultan datos,
                // no es necesario hacer seguimiento de los cambios.
                .AsNoTracking()

                // Muestra primero las proformas más recientes.
                .OrderByDescending(
                    p => p.FechaCreacion
                )

                // Ejecuta la consulta.
                .ToListAsync();
        }


        // =========================================================
        // OBTENER UNA PROFORMA
        // =========================================================

        // GET: api/Proformas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Proforma>>
            GetProforma(int id)
        {
            // Busca la proforma por su identificador.
            var proforma =
                await _context.Proformas.FindAsync(id);

            // Comprueba si la proforma existe.
            if (proforma == null)
            {
                return NotFound();
            }

            // Devuelve la proforma encontrada.
            return proforma;
        }


        // =========================================================
        // CREAR UNA PROFORMA
        // =========================================================

        // POST: api/Proformas
        [HttpPost]
        public async Task<ActionResult<Proforma>>
            PostProforma(Proforma proforma)
        {
            // Se coloca en cero para que la base de datos
            // genere automáticamente el identificador.
            proforma.IdProforma = 0;

            // Registra la fecha actual de creación.
            proforma.FechaCreacion = DateTime.Now;

            // Si no se recibe un estado,
            // se establece como Pendiente.
            if (
                string.IsNullOrWhiteSpace(
                    proforma.Estado
                )
            )
            {
                proforma.Estado = "Pendiente";
            }

            // Agrega la nueva proforma al contexto.
            _context.Proformas.Add(proforma);

            // Guarda la información en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve HTTP 201 indicando que
            // la proforma fue creada correctamente.
            return CreatedAtAction(
                nameof(GetProforma),

                // Envía el ID de la proforma creada.
                new
                {
                    id = proforma.IdProforma
                },

                // Devuelve la proforma creada.
                proforma
            );
        }


        // =========================================================
        // ACTUALIZAR UNA PROFORMA
        // =========================================================

        // PUT: api/Proformas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProforma(
            int id,
            Proforma proforma)
        {
            // Busca la proforma que se desea modificar.
            var proformaActual =
                await _context.Proformas.FindAsync(id);

            // Si no existe, devuelve HTTP 404.
            if (proformaActual == null)
            {
                return NotFound();
            }

            // Actualiza el usuario asociado.
            proformaActual.IdUsuario =
                proforma.IdUsuario;

            // Actualiza la dirección.
            proformaActual.IdDireccion =
                proforma.IdDireccion;

            // Actualiza la fecha de vencimiento.
            proformaActual.FechaVencimiento =
                proforma.FechaVencimiento;

            // Actualiza el subtotal.
            proformaActual.Subtotal =
                proforma.Subtotal;

            // Actualiza el impuesto.
            proformaActual.Impuesto =
                proforma.Impuesto;

            // Actualiza el descuento.
            proformaActual.Descuento =
                proforma.Descuento;

            // Actualiza el total.
            proformaActual.Total =
                proforma.Total;

            // Actualiza el estado de la proforma.
            proformaActual.Estado =
                proforma.Estado;

            // Actualiza la dirección del archivo PDF.
            proformaActual.UrlPdf =
                proforma.UrlPdf;

            // Guarda los cambios realizados.
            await _context.SaveChangesAsync();

            // Devuelve HTTP 204 indicando que
            // la actualización fue correcta.
            return NoContent();
        }


        // =========================================================
        // ELIMINAR UNA PROFORMA
        // =========================================================

        // DELETE: api/Proformas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult>
            DeleteProforma(int id)
        {
            // Busca la proforma por su identificador.
            var proforma =
                await _context.Proformas.FindAsync(id);

            // Comprueba si la proforma existe.
            if (proforma == null)
            {
                return NotFound();
            }

            // Busca los detalles relacionados
            // con la proforma.
            var detalles =
                await _context.DetalleProformas

                    // Filtra solamente los detalles
                    // de la proforma seleccionada.
                    .Where(
                        d => d.IdProforma == id
                    )

                    // Obtiene los detalles encontrados.
                    .ToListAsync();

            // Comprueba si existen detalles relacionados.
            if (detalles.Count > 0)
            {
                // Elimina todos los detalles antes de eliminar
                // la proforma principal.
                _context.DetalleProformas
                    .RemoveRange(detalles);
            }

            // Marca la proforma para eliminarla.
            _context.Proformas.Remove(proforma);

            // Guarda la eliminación en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve HTTP 204 indicando que
            // la eliminación fue correcta.
            return NoContent();
        }
    }
}