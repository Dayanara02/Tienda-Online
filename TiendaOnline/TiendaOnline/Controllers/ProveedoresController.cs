// Permite crear controladores y manejar respuestas HTTP.
using Microsoft.AspNetCore.Mvc;

// Permite utilizar Entity Framework Core para trabajar
// con la base de datos.
using Microsoft.EntityFrameworkCore;

// Permite utilizar atributos de autorización como [Authorize].
using Microsoft.AspNetCore.Authorization;

// Importa el contexto de la base de datos.
using TiendaOnline.AccesoDatos.Context;

// Importa las entidades del proyecto.
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers
{
    // Define la ruta principal:
    // api/Proveedores
    [Route("api/[controller]")]

    // Indica que esta clase funciona como controlador de API.
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        // Contexto utilizado para consultar y modificar
        // información en la base de datos.
        private readonly TiendaOnlineContext _context;

        // Constructor del controlador.
        public ProveedoresController(
            TiendaOnlineContext context)
        {
            // Guarda el contexto recibido.
            _context = context;
        }


        // =========================================================
        // OBTENER TODOS LOS PROVEEDORES
        // =========================================================

        // GET: api/Proveedores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor>>>
            GetProveedores()
        {
            // Obtiene todos los proveedores registrados.
            return await _context.Proveedors
                .ToListAsync();
        }


        // =========================================================
        // OBTENER UN PROVEEDOR
        // =========================================================

        // GET: api/Proveedores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Proveedor>>
            GetProveedor(int id)
        {
            // Busca el proveedor por su identificador.
            var proveedor =
                await _context.Proveedors.FindAsync(id);

            // Comprueba si el proveedor existe.
            if (proveedor == null)
                return NotFound();

            // Devuelve el proveedor encontrado.
            return proveedor;
        }


        // =========================================================
        // CREAR UN PROVEEDOR
        // =========================================================

        // Solo los Administradores pueden crear proveedores.
        [Authorize(Roles = "Administrador")]

        // POST: api/Proveedores
        [HttpPost]
        public async Task<ActionResult<Proveedor>>
            PostProveedor(Proveedor proveedor)
        {
            // Se coloca en cero para que la base de datos
            // genere automáticamente el identificador.
            proveedor.IdProveedor = 0;

            // Agrega el proveedor al contexto.
            _context.Proveedors.Add(proveedor);

            // Guarda el nuevo proveedor en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve HTTP 201 indicando que se creó correctamente.
            return CreatedAtAction(
                nameof(GetProveedor),

                // Envía el ID del proveedor creado.
                new
                {
                    id = proveedor.IdProveedor
                },

                // Devuelve los datos del proveedor creado.
                proveedor
            );
        }


        // =========================================================
        // ACTUALIZAR UN PROVEEDOR
        // =========================================================

        // Solo los Administradores pueden modificar proveedores.
        [Authorize(Roles = "Administrador")]

        // PUT: api/Proveedores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProveedor(
            int id,
            Proveedor proveedor)
        {
            // Comprueba que el ID de la URL
            // coincida con el ID enviado.
            if (id != proveedor.IdProveedor)
                return BadRequest();

            // Busca el proveedor existente.
            var existente =
                await _context.Proveedors.FindAsync(id);

            // Si no existe, devuelve HTTP 404.
            if (existente == null)
                return NotFound();

            // Actualiza el nombre del proveedor.
            existente.Nombre =
                proveedor.Nombre;

            // Actualiza la identificación.
            existente.Identificacion =
                proveedor.Identificacion;

            // Actualiza el correo electrónico.
            existente.Correo =
                proveedor.Correo;

            // Actualiza el número de teléfono.
            existente.Telefono =
                proveedor.Telefono;

            // Actualiza la dirección.
            existente.Direccion =
                proveedor.Direccion;

            // Actualiza el estado del proveedor.
            existente.Estado =
                proveedor.Estado;

            // Guarda los cambios en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve HTTP 204 indicando
            // que la actualización fue correcta.
            return NoContent();
        }


        // =========================================================
        // ELIMINAR UN PROVEEDOR
        // =========================================================

        // Solo los Administradores pueden eliminar proveedores.
        [Authorize(Roles = "Administrador")]

        // DELETE: api/Proveedores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult>
            DeleteProveedor(int id)
        {
            // Busca el proveedor por su identificador.
            var proveedor =
                await _context.Proveedors.FindAsync(id);

            // Comprueba si el proveedor existe.
            if (proveedor == null)
                return NotFound();

            // Marca el proveedor para eliminarlo.
            _context.Proveedors.Remove(proveedor);

            // Guarda la eliminación en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve HTTP 204 indicando
            // que el proveedor fue eliminado.
            return NoContent();
        }
    }
}