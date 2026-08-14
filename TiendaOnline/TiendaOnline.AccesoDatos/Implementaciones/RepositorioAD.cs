using Microsoft.EntityFrameworkCore; // Permite utilizar Entity Framework Core para trabajar con la base de datos.
using System.Linq.Expressions; // Permite utilizar expresiones para crear condiciones de búsqueda.
using TiendaOnline.Dominio.InterfacesAD; // Contiene la interfaz que implementa este repositorio.
using TiendaOnline.Utilidades; // Contiene la clase Respuesta utilizada para devolver resultados.

namespace TiendaOnline.AccesoDatos.Implementaciones
{
    // Clase que funciona como un repositorio genérico para trabajar con diferentes entidades.
    public class RepositorioAD<TEntity> : IRepositorioAD<TEntity>
        // Indica que TEntity debe ser una clase.
        where TEntity : class
    {
        // Guarda una referencia al contexto de Entity Framework.
        protected readonly DbContext _context;

        // Constructor que recibe el contexto de la base de datos.
        public RepositorioAD(DbContext context)
        {
            // Guarda el contexto recibido para utilizarlo en los métodos del repositorio.
            _context = context;
        }

        // Método que permite insertar una nueva entidad en la base de datos.
        public async Task<Respuesta<TEntity>> InsertarAsync(TEntity objEntidad)
        {
            // Crea una respuesta que contendrá la entidad insertada o el error.
            Respuesta<TEntity> objRespuesta = new Respuesta<TEntity>();

            // Inicia el bloque para controlar posibles errores.
            try
            {
                // Obtiene la tabla correspondiente a la entidad y agrega el nuevo registro.
                await _context.Set<TEntity>().AddAsync(objEntidad);

                // Guarda los cambios realizados en la base de datos.
                await _context.SaveChangesAsync();

                // Guarda la entidad insertada en la respuesta.
                objRespuesta.Data = objEntidad;
            }
            // Captura cualquier error que ocurra durante la inserción.
            catch (Exception ex)
            {
                // Guarda el mensaje del error en la respuesta.
                objRespuesta.Error = ex.Message;

                // Indica que no se obtuvo una entidad como resultado.
                objRespuesta.Data = null;
            }

            // Devuelve la respuesta de la operación.
            return objRespuesta;
        }

        // Método que permite modificar una entidad existente.
        public async Task<Respuesta<TEntity>> ModificarAsync(TEntity objEntidad)
        {
            // Crea una respuesta que contendrá la entidad modificada o el error.
            Respuesta<TEntity> objRespuesta = new Respuesta<TEntity>();

            // Inicia el bloque para controlar posibles errores.
            try
            {
                // Indica a Entity Framework que la entidad contiene cambios que deben guardarse.
                _context.Set<TEntity>().Update(objEntidad);

                // Guarda los cambios realizados en la base de datos.
                await _context.SaveChangesAsync();

                // Guarda la entidad modificada en la respuesta.
                objRespuesta.Data = objEntidad;
            }
            // Captura cualquier error ocurrido durante la modificación.
            catch (Exception ex)
            {
                // Guarda el mensaje del error en la respuesta.
                objRespuesta.Error = ex.Message;

                // Indica que no se obtuvo una entidad modificada.
                objRespuesta.Data = null;
            }

            // Devuelve la respuesta de la operación.
            return objRespuesta;
        }

        // Método que permite eliminar una entidad de la base de datos.
        public async Task<Respuesta<bool>> EliminarAsync(TEntity objEntidad)
        {
            // Crea una respuesta que indicará si la eliminación fue exitosa.
            Respuesta<bool> objRespuesta = new Respuesta<bool>();

            // Inicia el bloque para controlar posibles errores.
            try
            {
                // Marca la entidad como eliminada dentro del contexto de Entity Framework.
                _context.Entry(objEntidad).State = EntityState.Deleted;

                // Guarda los cambios y ejecuta la eliminación en la base de datos.
                await _context.SaveChangesAsync();

                // Indica que la eliminación se realizó correctamente.
                objRespuesta.Data = true;
            }
            // Captura cualquier error ocurrido durante la eliminación.
            catch (Exception ex)
            {
                // Guarda el mensaje del error en la respuesta.
                objRespuesta.Error = ex.Message;

                // Indica que la eliminación no fue exitosa.
                objRespuesta.Data = false;
            }

            // Devuelve el resultado de la eliminación.
            return objRespuesta;
        }

        // Método que permite obtener todos los registros de una entidad.
        public async Task<Respuesta<IEnumerable<TEntity>>> ListarAsync(
            List<string>? objIncludes = null)
        {
            // Crea una respuesta que contendrá la lista de registros.
            Respuesta<IEnumerable<TEntity>> objRespuesta =
                new Respuesta<IEnumerable<TEntity>>();

            // Inicia el bloque para controlar posibles errores.
            try
            {
                // Obtiene la tabla correspondiente a la entidad.
                IQueryable<TEntity> objPreconsulta =
                    _context.Set<TEntity>();

                // Verifica si se enviaron relaciones que también deben consultarse.
                if (objIncludes != null)
                {
                    // Recorre cada relación recibida.
                    objIncludes.ForEach(
                        x =>
                        {
                            // Agrega la relación a la consulta utilizando Include.
                            objPreconsulta =
                                objPreconsulta.Include(x);
                        });
                }

                // Ejecuta la consulta y obtiene todos los registros.
                objRespuesta.Data =
                    await objPreconsulta.ToListAsync();
            }
            // Captura cualquier error ocurrido durante la consulta.
            catch (Exception ex)
            {
                // Guarda el mensaje del error en la respuesta.
                objRespuesta.Error = ex.Message;

                // Indica que no se pudo obtener la lista.
                objRespuesta.Data = null;
            }

            // Devuelve la lista de registros o el error.
            return objRespuesta;
        }

        // Método que permite buscar registros utilizando una condición.
        public async Task<Respuesta<IEnumerable<TEntity>>> BuscarAsync(
            Expression<Func<TEntity, bool>> objPredicado,
            List<string>? objIncludes = null)
        {
            // Crea una respuesta que contendrá los registros encontrados.
            Respuesta<IEnumerable<TEntity>> objRespuesta =
                new Respuesta<IEnumerable<TEntity>>();

            // Inicia el bloque para controlar posibles errores.
            try
            {
                // Obtiene la tabla correspondiente a la entidad.
                IQueryable<TEntity> objPreconsulta =
                    _context.Set<TEntity>();

                // Verifica si existen relaciones que deben ser incluidas.
                if (objIncludes != null)
                {
                    // Recorre cada relación recibida.
                    objIncludes.ForEach(
                        x =>
                        {
                            // Agrega la relación a la consulta.
                            objPreconsulta =
                                objPreconsulta.Include(x);
                        });
                }

                // Aplica la condición de búsqueda y obtiene los registros encontrados.
                objRespuesta.Data =
                    await objPreconsulta
                        .Where(objPredicado)
                        .ToListAsync();
            }
            // Captura cualquier error ocurrido durante la búsqueda.
            catch (Exception ex)
            {
                // Guarda el mensaje del error en la respuesta.
                objRespuesta.Error = ex.Message;

                // Indica que no se pudieron obtener los registros.
                objRespuesta.Data = null;
            }

            // Devuelve los registros encontrados o el error.
            return objRespuesta;
        }

        // Método que permite obtener una sola entidad que cumpla una condición.
        public async Task<Respuesta<TEntity>> ObtenerEntidadAsync(
            Expression<Func<TEntity, bool>> objPredicado,
            List<string>? objIncludes = null)
        {
            // Crea una respuesta que contendrá la entidad encontrada.
            Respuesta<TEntity> objRespuesta =
                new Respuesta<TEntity>();

            // Inicia el bloque para controlar posibles errores.
            try
            {
                // Obtiene la tabla correspondiente a la entidad.
                IQueryable<TEntity> objPreconsulta =
                    _context.Set<TEntity>();

                // Verifica si se enviaron relaciones que deben ser incluidas.
                if (objIncludes != null)
                {
                    // Recorre las relaciones recibidas.
                    objIncludes.ForEach(
                        x =>
                        {
                            // Agrega cada relación a la consulta.
                            objPreconsulta =
                                objPreconsulta.Include(x);
                        });
                }

                // Aplica la condición y obtiene el primer registro encontrado.
                objRespuesta.Data =
                    await objPreconsulta
                        .Where(objPredicado)
                        .FirstOrDefaultAsync();
            }
            // Captura cualquier error ocurrido durante la consulta.
            catch (Exception ex)
            {
                // Guarda el mensaje del error en la respuesta.
                objRespuesta.Error = ex.Message;

                // Indica que no se obtuvo una entidad.
                objRespuesta.Data = null;
            }

            // Devuelve la entidad encontrada o el error.
            return objRespuesta;
        }

        // Método que permite contar los registros que cumplen una condición.
        public async Task<Respuesta<int?>> ContarAsync(
            Expression<Func<TEntity, bool>> objPredicado,
            List<string>? objIncludes = null)
        {
            // Crea una respuesta que contendrá la cantidad de registros.
            Respuesta<int?> objRespuesta =
                new Respuesta<int?>();

            // Inicia el bloque para controlar posibles errores.
            try
            {
                // Obtiene la tabla correspondiente a la entidad.
                IQueryable<TEntity> objPreconsulta =
                    _context.Set<TEntity>();

                // Verifica si se enviaron relaciones para incluir.
                if (objIncludes != null)
                {
                    // Recorre cada relación recibida.
                    objIncludes.ForEach(
                        x =>
                        {
                            // Agrega la relación a la consulta.
                            objPreconsulta =
                                objPreconsulta.Include(x);
                        });
                }

                // Cuenta los registros que cumplen con la condición indicada.
                objRespuesta.Data =
                    await objPreconsulta.CountAsync(objPredicado);
            }
            // Captura cualquier error ocurrido durante el conteo.
            catch (Exception ex)
            {
                // Guarda el mensaje del error en la respuesta.
                objRespuesta.Error = ex.Message;

                // Indica que no se pudo obtener el conteo.
                objRespuesta.Data = null;
            }

            // Devuelve la cantidad de registros o el error.
            return objRespuesta;
        }
    }
}