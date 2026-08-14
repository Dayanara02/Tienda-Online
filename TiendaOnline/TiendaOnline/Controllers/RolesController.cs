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
    // Solo los usuarios con rol Administrador
    // pueden acceder a este controlador.
    [Authorize(Roles = "Administrador")]

    // Define la ruta principal:
    // api/Roles
    [Route("api/[controller]")]

    // Indica que esta clase funciona como controlador de API.
    [ApiController]
    public class RolesController : ControllerBase
    {
        // Contexto utilizado para consultar y modificar
        // los roles en la base de datos.
        private readonly TiendaOnlineContext _context;

        // Constructor del controlador.
        public RolesController(
            TiendaOnlineContext context)
        {
            // Guarda el contexto recibido.
            _context = context;
        }


        // =========================================================
        // OBTENER TODOS LOS ROLES
        // =========================================================

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>>
            GetRoles()
        {
            // Obtiene todos los roles registrados.
            return await _context.Rols
                .ToListAsync();
        }


        // =========================================================
        // OBTENER UN ROL
        // =========================================================

        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rol>>
            GetRol(int id)
        {
            // Busca el rol por su identificador.
            var rol =
                await _context.Rols.FindAsync(id);

            // Comprueba si el rol existe.
            if (rol == null)
                return NotFound();

            // Devuelve el rol encontrado.
            return rol;
        }


        // =========================================================
        // CREAR UN ROL
        // =========================================================

        // POST: api/Roles
        [HttpPost]
        public async Task<ActionResult<Rol>>
            PostRol(Rol rol)
        {
            // Agrega el nuevo rol al contexto.
            _context.Rols.Add(rol);

            // Guarda el rol en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve HTTP 201 indicando
            // que el rol fue creado correctamente.
            return CreatedAtAction(
                nameof(GetRol),

                // Envía el identificador del rol creado.
                new
                {
                    id = rol.IdRol
                },

                // Devuelve los datos del rol.
                rol
            );
        }


        // =========================================================
        // ACTUALIZAR UN ROL
        // =========================================================

        // PUT: api/Roles/5
        [HttpPut("{id}")]
        public async Task<IActionResult>
            PutRol(
                int id,
                Rol rol)
        {
            // Comprueba que el ID de la URL
            // coincida con el ID del objeto recibido.
            if (id != rol.IdRol)
                return BadRequest();

            // Indica a Entity Framework que el rol
            // debe actualizarse.
            _context.Entry(rol).State =
                EntityState.Modified;

            // Guarda los cambios realizados.
            await _context.SaveChangesAsync();

            // Devuelve HTTP 204 indicando
            // que la actualización fue correcta.
            return NoContent();
        }


        // =========================================================
        // ELIMINAR UN ROL
        // =========================================================

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult>
            DeleteRol(int id)
        {
            // Busca el rol por su identificador.
            var rol =
                await _context.Rols.FindAsync(id);

            // Comprueba si el rol existe.
            if (rol == null)
                return NotFound();

            // Marca el rol para eliminarlo.
            _context.Rols.Remove(rol);

            // Guarda la eliminación en la base de datos.
            await _context.SaveChangesAsync();

            // Devuelve HTTP 204 indicando
            // que el rol fue eliminado correctamente.
            return NoContent();
        }
    }
}