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

// Solo los usuarios con rol Administrador
// pueden acceder a este controlador.
[Authorize(Roles = "Administrador")]

// Define la ruta principal:
// api/Usuarios
[Route("api/[controller]")]

// Indica que esta clase funciona como controlador de API.
[ApiController]
public class UsuariosController : ControllerBase
{
    // Contexto utilizado para consultar y modificar
    // los usuarios en la base de datos.
    private readonly TiendaOnlineContext _context;

    // Constructor del controlador.
    public UsuariosController(TiendaOnlineContext context)
    {
        // Guarda el contexto recibido.
        _context = context;
    }

    // GET: api/Usuarios
    // Obtiene todos los usuarios registrados.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
    {
        // Realiza una consulta de solo lectura
        // para obtener todos los usuarios.
        return await _context.Usuarios
            .AsNoTracking()
            .ToListAsync();
    }

    // GET: api/Usuarios/5
    // Obtiene un usuario específico por su ID.
    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> GetUsuario(int id)
    {
        // Busca el usuario por su identificador.
        var usuario = await _context.Usuarios.FindAsync(id);

        // Si no existe, devuelve HTTP 404.
        if (usuario == null)
            return NotFound();

        // Devuelve el usuario encontrado.
        return usuario;
    }

    // POST: api/Usuarios
    // Registra un nuevo usuario.
    [HttpPost]
    public async Task<ActionResult<Usuario>> PostUsuario(
        Usuario usuario)
    {
        // Se coloca en cero para que la base de datos
        // genere automáticamente el identificador.
        usuario.IdUsuario = 0;

        // Agrega el usuario al contexto.
        _context.Usuarios.Add(usuario);

        // Guarda el nuevo usuario en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve HTTP 201 indicando que fue creado.
        return CreatedAtAction(
            nameof(GetUsuario),
            new
            {
                id = usuario.IdUsuario
            },
            usuario
        );
    }

    // PUT: api/Usuarios/5
    // Actualiza la información de un usuario.
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUsuario(
        int id,
        Usuario usuario)
    {
        // Comprueba que el ID de la URL
        // coincida con el ID del usuario recibido.
        if (id != usuario.IdUsuario)
            return BadRequest();

        // Busca el usuario existente.
        var existente =
            await _context.Usuarios.FindAsync(id);

        // Si no existe, devuelve HTTP 404.
        if (existente == null)
            return NotFound();

        // Actualiza el rol del usuario.
        existente.IdRol = usuario.IdRol;

        // Actualiza el nombre.
        existente.Nombre = usuario.Nombre;

        // Actualiza el apellido.
        existente.Apellido = usuario.Apellido;

        // Actualiza el correo electrónico.
        existente.Correo = usuario.Correo;

        // Actualiza la contraseña.
        existente.Contrasena = usuario.Contrasena;

        // Actualiza el teléfono.
        existente.Telefono = usuario.Telefono;

        // Actualiza el estado del usuario.
        existente.Estado = usuario.Estado;

        // Guarda los cambios en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve HTTP 204 indicando
        // que la actualización fue correcta.
        return NoContent();
    }

    // DELETE: api/Usuarios/5
    // Elimina un usuario por su ID.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUsuario(int id)
    {
        // Busca el usuario por su identificador.
        var usuario =
            await _context.Usuarios.FindAsync(id);

        // Si no existe, devuelve HTTP 404.
        if (usuario == null)
            return NotFound();

        // Marca el usuario para eliminarlo.
        _context.Usuarios.Remove(usuario);

        // Guarda la eliminación en la base de datos.
        await _context.SaveChangesAsync();

        // Devuelve HTTP 204 indicando
        // que el usuario fue eliminado correctamente.
        return NoContent();
    }
}