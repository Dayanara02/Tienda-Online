// Permite utilizar autorización por roles.
using Microsoft.AspNetCore.Authorization;

// Permite crear controladores de API.
using Microsoft.AspNetCore.Mvc;

// Permite realizar consultas con Entity Framework Core.
using Microsoft.EntityFrameworkCore;

// Importa el contexto de la base de datos.
using TiendaOnline.AccesoDatos.Context;

// Importa la entidad Producto.
using TiendaOnline.Dominio.Entidades;

// Define el espacio de nombres del controlador.
namespace TiendaOnline.API.Controllers;

// Indica que esta clase funciona como controlador de API.
[ApiController]

// Define la ruta principal como api/Productos.
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    // Guarda el contexto utilizado para acceder a SQL Server.
    private readonly TiendaOnlineContext _context;

    // Recibe el contexto mediante inyección de dependencias.
    public ProductosController(
        TiendaOnlineContext context
    )
    {
        // Guarda el contexto recibido.
        _context = context;
    }

    // Obtiene todos los productos activos de la tienda.
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetProductos()
    {
        // Consulta los productos activos.
        var productos =
            await _context.Productos

                // La consulta es solamente de lectura.
                .AsNoTracking()

                // Filtra solamente productos activos.
                .Where(
                    producto =>
                        producto.Estado
                )

                // Ordena los productos por nombre.
                .OrderBy(
                    producto =>
                        producto.Nombre
                )

                // Selecciona solamente los datos que necesita Angular.
                .Select(
                    producto => new
                    {
                        // Identificador real del producto.
                        idProducto =
                            producto.IdProducto,

                        // Identificador de la categoría.
                        idCategoria =
                            producto.IdCategoria,

                        // Nombre real de la categoría.
                        categoria =
                            producto.IdCategoriaNavigation != null
                                ? producto.IdCategoriaNavigation.Nombre
                                : "Sin categoría",

                        // Nombre real del producto.
                        nombre =
                            producto.Nombre,

                        // Descripción registrada en SQL Server.
                        descripcion =
                            producto.Descripcion,

                        // Precio real guardado en SQL Server.
                        precio =
                            producto.Precio,

                        // Imagen registrada para el producto.
                        imagen =
                            producto.Imagen,

                        // Stock real obtenido desde Inventario.
                        stock =
                            producto.Inventario != null
                                ? producto.Inventario.CantidadDisponible
                                : 0
                    }
                )

                // Ejecuta la consulta.
                .ToListAsync();

        // Devuelve HTTP 200 con la lista.
        return Ok(productos);
    }

    // Obtiene un producto específico por su identificador.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProducto(
        int id
    )
    {
        // Busca el producto solicitado.
        var producto =
            await _context.Productos

                // La consulta es solamente de lectura.
                .AsNoTracking()

                // Busca por identificador.
                .Where(
                    producto =>
                        producto.IdProducto == id
                )

                // Selecciona los datos que necesita Angular.
                .Select(
                    producto => new
                    {
                        // Identificador real.
                        idProducto =
                            producto.IdProducto,

                        // Identificador de la categoría.
                        idCategoria =
                            producto.IdCategoria,

                        // Nombre real de la categoría.
                        categoria =
                            producto.IdCategoriaNavigation != null
                                ? producto.IdCategoriaNavigation.Nombre
                                : "Sin categoría",

                        // Nombre del producto.
                        nombre =
                            producto.Nombre,

                        // Descripción del producto.
                        descripcion =
                            producto.Descripcion,

                        // Precio actual.
                        precio =
                            producto.Precio,

                        // Imagen del producto.
                        imagen =
                            producto.Imagen,

                        // Stock actual.
                        stock =
                            producto.Inventario != null
                                ? producto.Inventario.CantidadDisponible
                                : 0,

                        // Indica si el producto está activo.
                        estado =
                            producto.Estado
                    }
                )

                // Obtiene un único resultado.
                .FirstOrDefaultAsync();

        // Comprueba si el producto existe.
        if (producto == null)
        {
            // Devuelve HTTP 404.
            return NotFound(
                "El producto no existe."
            );
        }

        // Devuelve HTTP 200 con el producto.
        return Ok(producto);
    }

    // Permite al Administrador crear un producto.
    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<ActionResult<Producto>> PostProducto(
        Producto producto
    )
    {
        // Guarda la fecha de registro.
        producto.FechaRegistro =
            DateTime.Now;

        // Agrega el producto al contexto.
        _context.Productos.Add(
            producto
        );

        // Guarda el nuevo producto.
        await _context
            .SaveChangesAsync();

        // Devuelve HTTP 201.
        return CreatedAtAction(
            nameof(GetProducto),
            new
            {
                id =
                    producto.IdProducto
            },
            producto
        );
    }

    // Permite al Administrador modificar un producto.
    [Authorize(Roles = "Administrador")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutProducto(
        int id,
        Producto producto
    )
    {
        // Comprueba que los identificadores coincidan.
        if (
            id !=
            producto.IdProducto
        )
        {
            // Devuelve HTTP 400.
            return BadRequest(
                "El ID de la URL no coincide con el ID del producto."
            );
        }

        // Marca la entidad como modificada.
        _context.Entry(
            producto
        ).State =
            EntityState.Modified;

        try
        {
            // Guarda los cambios.
            await _context
                .SaveChangesAsync();
        }
        catch (
            DbUpdateConcurrencyException
        )
        {
            // Comprueba si el producto todavía existe.
            if (
                !ProductoExists(id)
            )
            {
                // Devuelve HTTP 404.
                return NotFound();
            }

            // Vuelve a lanzar el error.
            throw;
        }

        // Devuelve HTTP 204.
        return NoContent();
    }

    // Permite al Administrador eliminar un producto.
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProducto(
        int id
    )
    {
        // Busca el producto solicitado.
        var producto =
            await _context.Productos
                .FindAsync(id);

        // Comprueba que exista.
        if (producto == null)
        {
            // Devuelve HTTP 404.
            return NotFound();
        }

        // Elimina el producto.
        _context.Productos.Remove(
            producto
        );

        // Guarda la eliminación.
        await _context
            .SaveChangesAsync();

        // Devuelve HTTP 204.
        return NoContent();
    }

    // Comprueba si existe un producto determinado.
    private bool ProductoExists(
        int id
    )
    {
        // Busca el identificador en la tabla Producto.
        return _context.Productos
            .Any(
                producto =>
                    producto.IdProducto == id
            );
    }
}