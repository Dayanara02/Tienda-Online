using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaOnline.AccesoDatos.Context;
using Microsoft.AspNetCore.Authorization;
using TiendaOnline.Dominio.Entidades;

namespace TiendaOnline.API.Controllers
{   //Define la ruta y las funcionalidades de la API para trabajar con las categorias
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {   //Permite acceder a la base de datos mediante el contexto de TiendaOnline
        private readonly TiendaOnlineContext _context;

        //Constructor que recibe el contexto de la base de datos mediante inyeccion de dependencias 
        public CategoriasController(TiendaOnlineContext context)
        {
            _context = context;
        }
        //Metodo GET para obtener todas las categorias registradas 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categorium>>> GetCategorias()
        {   //Obtiene todas las categorias de la base de datos y las convierte en una lista
            return await _context.Categoria.ToListAsync();
        }

        //Metodo GET que permite obtener una categoria especifica utilizando su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Categorium>> GetCategoria(int id)
        {   //Busca la categoria en la base de datos utilizando el ID proporcionado
            var categoria = await _context.Categoria.FindAsync(id);

            if (categoria == null)
                return NotFound();

            return categoria;
        }
        //Solo los usuarios que tengan el rol de Administrador pueden crear categorias 
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<ActionResult<Categorium>> PostCategoria(
            Categorium categoria)
        {
            categoria.IdCategoria = 0;

            _context.Categoria.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategoria),
                new { id = categoria.IdCategoria },
                categoria
            );
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(
            int id,
            Categorium categoria)
        {
            if (id != categoria.IdCategoria)
                return BadRequest();
            //Busca la categoria que se desea eliminar
            var existente = await _context.Categoria.FindAsync(id);
            //Si la categoria no existe, devuelve una repuesta NotFound
            if (existente == null)
                return NotFound();

            existente.IdFamilia = categoria.IdFamilia;
            existente.Nombre = categoria.Nombre;
            existente.Descripcion = categoria.Descripcion;
            existente.Estado = categoria.Estado;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categoria.FindAsync(id);
            //Si la categoria no existe, devuelve una repuesta NotFound
            if (categoria == null)
                return NotFound();

            _context.Categoria.Remove(categoria);
            await _context.SaveChangesAsync();
            //Devuelve una repuesta indicando que la eliminacion fue exitosa
            return NoContent();
        }
    }
}