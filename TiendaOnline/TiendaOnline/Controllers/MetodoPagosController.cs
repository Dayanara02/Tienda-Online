// Permite crear controladores de API y devolver respuestas
// como Ok(), NotFound(), Conflict(), etc.
using Microsoft.AspNetCore.Mvc;

// Permite realizar consultas con Entity Framework Core,
// por ejemplo AsNoTracking(), Where() y ToListAsync().
using Microsoft.EntityFrameworkCore;

// Importa el contexto de la base de datos.
using TiendaOnline.AccesoDatos.Context;

// Permite proteger los endpoints según
// el usuario autenticado y su rol.
using Microsoft.AspNetCore.Authorization;

// Importa la entidad MetodoPago.
using TiendaOnline.Dominio.Entidades;


namespace TiendaOnline.API.Controllers;


// Indica que para entrar a este controlador
// el usuario debe haber iniciado sesión.
[Authorize]

// Indica que esta clase funciona como controlador de API.
[ApiController]

// Define la ruta principal:
//
// api/MetodoPagos
[Route("api/[controller]")]
public class MetodoPagosController : ControllerBase
{
    // Guarda el contexto utilizado
    // para trabajar con SQL Server.
    private readonly TiendaOnlineContext _context;


    // Constructor del controlador.
    public MetodoPagosController(
        TiendaOnlineContext context
    )
    {
        // Guarda el contexto recibido
        // mediante inyección de dependencias.
        _context = context;
    }


    // =========================================================
    // OBTENER TODOS LOS MÉTODOS DE PAGO
    // =========================================================

    // GET: api/MetodoPagos
    //
    // Este endpoint es utilizado para administración.
    //
    // Devuelve métodos activos e inactivos.
    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MetodoPago>>>
        GetMetodoPagos()
    {
        // Consulta todos los métodos de pago.
        var metodos =
            await _context.MetodoPagos

                // Solamente se van a leer los registros.
                .AsNoTracking()

                // Ordena por nombre.
                .OrderBy(
                    m => m.Nombre
                )

                // Ejecuta la consulta.
                .ToListAsync();


        // Devuelve código HTTP 200
        // junto con los métodos encontrados.
        return Ok(metodos);
    }


    // =========================================================
    // OBTENER MÉTODOS DE PAGO DISPONIBLES PARA EL CLIENTE
    // =========================================================

    // GET: api/MetodoPagos/disponibles
    //
    // Este es el endpoint que utilizará
    // la pantalla de pago del Cliente.
    //
    // Solamente devuelve métodos que estén activos.
    [Authorize(Roles = "Cliente")]
    [HttpGet("disponibles")]
    public async Task<IActionResult>
        GetMetodosDisponibles()
    {
        // Consulta la tabla MetodoPago.
        var metodos =
            await _context.MetodoPagos

                // Solamente se necesita leer información.
                .AsNoTracking()

                // Filtra únicamente los métodos activos.
                .Where(
                    m => m.Estado
                )

                // Ordena los métodos por nombre.
                .OrderBy(
                    m => m.Nombre
                )

                // Devuelve solamente la información
                // que Angular necesita mostrar.
                .Select(
                    m => new
                    {
                        // Identificador del método.
                        idMetodoPago =
                            m.IdMetodoPago,

                        // Nombre visible.
                        nombre =
                            m.Nombre,

                        // Descripción opcional.
                        descripcion =
                            m.Descripcion
                    }
                )

                // Ejecuta la consulta.
                .ToListAsync();


        // Devuelve los métodos activos.
        return Ok(metodos);
    }


    // =========================================================
    // OBTENER UN MÉTODO DE PAGO
    // =========================================================

    // GET: api/MetodoPagos/5
    //
    // Solamente el Administrador
    // puede consultar un método individual
    // desde la parte administrativa.
    [Authorize(Roles = "Administrador")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MetodoPago>>
        GetMetodoPago(
            int id
        )
    {
        // Busca el método utilizando su identificador.
        var metodoPago =
            await _context.MetodoPagos
                .FindAsync(id);


        // Comprueba si existe.
        if (metodoPago == null)
        {
            // Devuelve HTTP 404.
            return NotFound(
                "El método de pago no existe."
            );
        }


        // Devuelve el método encontrado.
        return Ok(metodoPago);
    }


    // =========================================================
    // CREAR MÉTODO DE PAGO
    // =========================================================

    // POST: api/MetodoPagos
    //
    // Solamente el Administrador
    // puede crear nuevos métodos.
    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<ActionResult<MetodoPago>>
        PostMetodoPago(
            MetodoPago metodoPago
        )
    {
        // Asegura que SQL Server
        // genere automáticamente el identificador.
        metodoPago.IdMetodoPago = 0;


        // Comprueba si ya existe otro método
        // con el mismo nombre.
        var existe =
            await _context.MetodoPagos
                .AnyAsync(
                    m =>
                        m.Nombre ==
                        metodoPago.Nombre
                );


        // Evita guardar métodos duplicados.
        if (existe)
        {
            // HTTP 409 indica conflicto.
            return Conflict(
                "Ya existe un método de pago con ese nombre."
            );
        }


        // Agrega el nuevo método al contexto.
        _context.MetodoPagos.Add(
            metodoPago
        );


        // Guarda el registro
        // en la base de datos.
        await _context.SaveChangesAsync();


        // Devuelve HTTP 201
        // indicando que el registro fue creado.
        return CreatedAtAction(
            nameof(GetMetodoPago),
            new
            {
                id =
                    metodoPago.IdMetodoPago
            },
            metodoPago
        );
    }


    // =========================================================
    // MODIFICAR MÉTODO DE PAGO
    // =========================================================

    // PUT: api/MetodoPagos/5
    //
    // Solamente el Administrador
    // puede modificar métodos de pago.
    [Authorize(Roles = "Administrador")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult>
        PutMetodoPago(
            int id,
            MetodoPago metodoPago
        )
    {
        // Busca el método actual.
        var metodoActual =
            await _context.MetodoPagos
                .FindAsync(id);


        // Comprueba que exista.
        if (metodoActual == null)
        {
            return NotFound(
                "El método de pago no existe."
            );
        }


        // Actualiza el nombre.
        metodoActual.Nombre =
            metodoPago.Nombre;

        // Actualiza la descripción.
        metodoActual.Descripcion =
            metodoPago.Descripcion;

        // Actualiza si está activo o inactivo.
        metodoActual.Estado =
            metodoPago.Estado;


        // Guarda los cambios.
        await _context.SaveChangesAsync();


        // HTTP 204 indica que
        // la actualización terminó correctamente.
        return NoContent();
    }


    // =========================================================
    // ELIMINAR MÉTODO DE PAGO
    // =========================================================

    // DELETE: api/MetodoPagos/5
    //
    // Solamente el Administrador
    // puede eliminar métodos de pago.
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
        DeleteMetodoPago(
            int id
        )
    {
        // Busca el método.
        var metodoPago =
            await _context.MetodoPagos
                .FindAsync(id);


        // Comprueba que exista.
        if (metodoPago == null)
        {
            return NotFound(
                "El método de pago no existe."
            );
        }


        // Marca el registro para eliminarlo.
        _context.MetodoPagos.Remove(
            metodoPago
        );


        // Guarda la eliminación.
        await _context.SaveChangesAsync();


        // Devuelve HTTP 204.
        return NoContent();
    }
}