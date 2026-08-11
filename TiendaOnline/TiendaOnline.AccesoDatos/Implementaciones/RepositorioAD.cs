using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TiendaOnline.Dominio.InterfacesAD;
using TiendaOnline.Utilidades;

namespace TiendaOnline.AccesoDatos.Implementaciones
{
    // Esta clase funciona como un repositorio genérico.
    // Permite realizar operaciones básicas de base de datos con cualquier entidad.
    public class RepositorioAD<TEntity> : IRepositorioAD<TEntity>
        where TEntity : class
    {
        // Guarda una referencia al contexto de Entity Framework.
        // Se utiliza para acceder a las tablas y realizar operaciones en la base de datos.
        protected readonly DbContext _context;

        // Recibe el contexto desde la inyección de dependencias.
        // Esto permite que el repositorio utilice la conexión configurada en el proyecto.
        public RepositorioAD(DbContext context)
        {
            // Asigna el contexto recibido a la variable interna de la clase.
            _context = context;
        }

        // Inserta una nueva entidad en la base de datos.
        public async Task<Respuesta<TEntity>> InsertarAsync(TEntity objEntidad)
        {
            // Crea el objeto que se utilizará para devolver los datos o un error.
            Respuesta<TEntity> objRespuesta = new Respuesta<TEntity>();

            try
            {
                // Obtiene la tabla correspondiente a TEntity y agrega la nueva entidad.
                // AddAsync prepara el registro para ser insertado en la base de datos.
                await _context.Set<TEntity>().AddAsync(objEntidad);

                // Guarda en la base de datos todos los cambios pendientes en el contexto.
                await _context.SaveChangesAsync();

                // Guarda la entidad insertada dentro de la respuesta.
                objRespuesta.Data = objEntidad;
            }
            catch (Exception ex)
            {
                // Guarda el mensaje del error ocurrido durante la inserción.
                objRespuesta.Error = ex.Message;

                // Indica que no se pudo obtener una entidad como resultado.
                objRespuesta.Data = null;
            }

            // Devuelve el resultado de la operación.
            return objRespuesta;
        }

        // Modifica una entidad que ya existe en la base de datos.
        public async Task<Respuesta<TEntity>> ModificarAsync(TEntity objEntidad)
        {
            // Crea el objeto que devolverá el resultado de la modificación.
            Respuesta<TEntity> objRespuesta = new Respuesta<TEntity>();

            try
            {
                // Indica a Entity Framework que la entidad recibida contiene cambios.
                // Update prepara esos cambios para enviarlos a la base de datos.
                _context.Set<TEntity>().Update(objEntidad);

                // Guarda los cambios realizados en la base de datos.
                await _context.SaveChangesAsync();

                // Devuelve la entidad después de haber sido modificada.
                objRespuesta.Data = objEntidad;
            }
            catch (Exception ex)
            {
                // Guarda el mensaje del error si no se pudo modificar la entidad.
                objRespuesta.Error = ex.Message;

                // Deja los datos vacíos porque la operación no se completó.
                objRespuesta.Data = null;
            }

            // Devuelve el resultado de la operación.
            return objRespuesta;
        }

        // Elimina una entidad de la base de datos.
        public async Task<Respuesta<bool>> EliminarAsync(TEntity objEntidad)
        {
            // Crea una respuesta de tipo bool para indicar si se eliminó correctamente.
            Respuesta<bool> objRespuesta = new Respuesta<bool>();

            try
            {
                // Indica a Entity Framework que la entidad debe ser eliminada.
                // La eliminación todavía no ocurre hasta ejecutar SaveChangesAsync.
                _context.Entry(objEntidad).State = EntityState.Deleted;

                // Ejecuta la eliminación realmente en la base de datos.
                await _context.SaveChangesAsync();

                // Indica que la eliminación fue realizada correctamente.
                objRespuesta.Data = true;
            }
            catch (Exception ex)
            {
                // Guarda el mensaje del error ocurrido al intentar eliminar.
                objRespuesta.Error = ex.Message;

                // Indica que la entidad no pudo ser eliminada.
                objRespuesta.Data = false;
            }

            // Devuelve si la eliminación fue correcta o no.
            return objRespuesta;
        }

        // Obtiene todos los registros de una entidad.
        public async Task<Respuesta<IEnumerable<TEntity>>> ListarAsync(
            List<string>? objIncludes = null)
        {
            // Crea una respuesta que puede contener una lista de entidades.
            Respuesta<IEnumerable<TEntity>> objRespuesta =
                new Respuesta<IEnumerable<TEntity>>();

            try
            {
                // Obtiene la tabla correspondiente a la entidad genérica TEntity.
                // IQueryable permite seguir agregando condiciones antes de ejecutar la consulta.
                IQueryable<TEntity> objPreconsulta =
                    _context.Set<TEntity>();

                // Verifica si se enviaron relaciones que también deben cargarse.
                if (objIncludes != null)
                {
                    // Recorre cada relación indicada.
                    objIncludes.ForEach(
                        x =>
                        {
                            // Include permite traer también datos relacionados.
                            // Por ejemplo, un producto junto con su categoría.
                            objPreconsulta =
                                objPreconsulta.Include(x);
                        });
                }

                // Ejecuta la consulta y convierte los resultados en una lista.
                objRespuesta.Data =
                    await objPreconsulta.ToListAsync();
            }
            catch (Exception ex)
            {
                // Guarda el mensaje del error si la consulta falla.
                objRespuesta.Error = ex.Message;

                // Deja los datos vacíos porque no se pudo obtener la lista.
                objRespuesta.Data = null;
            }

            // Devuelve la lista obtenida o el error.
            return objRespuesta;
        }

        // Busca registros que cumplan con una condición específica.
        public async Task<Respuesta<IEnumerable<TEntity>>> BuscarAsync(
            Expression<Func<TEntity, bool>> objPredicado,
            List<string>? objIncludes = null)
        {
            // Crea una respuesta que puede contener varios registros encontrados.
            Respuesta<IEnumerable<TEntity>> objRespuesta =
                new Respuesta<IEnumerable<TEntity>>();

            try
            {
                // Obtiene la tabla correspondiente a la entidad.
                IQueryable<TEntity> objPreconsulta =
                    _context.Set<TEntity>();

                // Verifica si se deben incluir datos relacionados.
                if (objIncludes != null)
                {
                    // Recorre las relaciones enviadas.
                    objIncludes.ForEach(
                        x =>
                        {
                            // Agrega cada relación a la consulta.
                            objPreconsulta =
                                objPreconsulta.Include(x);
                        });
                }

                // Where aplica la condición recibida en objPredicado.
                // ToListAsync ejecuta la consulta y devuelve todos los registros que cumplen.
                objRespuesta.Data =
                    await objPreconsulta
                        .Where(objPredicado)
                        .ToListAsync();
            }
            catch (Exception ex)
            {
                // Guarda el mensaje del error si la búsqueda falla.
                objRespuesta.Error = ex.Message;

                // Deja los datos vacíos al no poder completar la búsqueda.
                objRespuesta.Data = null;
            }

            // Devuelve los registros encontrados o el error.
            return objRespuesta;
        }

        // Obtiene una sola entidad que cumpla con una condición.
        public async Task<Respuesta<TEntity>> ObtenerEntidadAsync(
            Expression<Func<TEntity, bool>> objPredicado,
            List<string>? objIncludes = null)
        {
            // Crea una respuesta para devolver una sola entidad.
            Respuesta<TEntity> objRespuesta =
                new Respuesta<TEntity>();

            try
            {
                // Obtiene la tabla correspondiente a la entidad.
                IQueryable<TEntity> objPreconsulta =
                    _context.Set<TEntity>();

                // Verifica si también deben cargarse relaciones.
                if (objIncludes != null)
                {
                    // Agrega las relaciones solicitadas a la consulta.
                    objIncludes.ForEach(
                        x =>
                        {
                            // Include permite cargar datos relacionados con la entidad.
                            objPreconsulta =
                                objPreconsulta.Include(x);
                        });
                }

                // Where aplica la condición enviada.
                // FirstOrDefaultAsync obtiene el primer registro encontrado o null si no existe.
                objRespuesta.Data =
                    await objPreconsulta
                        .Where(objPredicado)
                        .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // Guarda el mensaje del error ocurrido al consultar la entidad.
                objRespuesta.Error = ex.Message;

                // Deja los datos vacíos porque no se encontró un resultado válido.
                objRespuesta.Data = null;
            }

            // Devuelve la entidad encontrada o el error.
            return objRespuesta;
        }

        // Cuenta cuántos registros cumplen con una condición.
        public async Task<Respuesta<int?>> ContarAsync(
            Expression<Func<TEntity, bool>> objPredicado,
            List<string>? objIncludes = null)
        {
            // Crea una respuesta que devolverá la cantidad de registros encontrados.
            Respuesta<int?> objRespuesta =
                new Respuesta<int?>();

            try
            {
                // Obtiene la tabla correspondiente a la entidad.
                IQueryable<TEntity> objPreconsulta =
                    _context.Set<TEntity>();

                // Verifica si se enviaron relaciones para incluir.
                if (objIncludes != null)
                {
                    // Agrega cada relación a la consulta.
                    objIncludes.ForEach(
                        x =>
                        {
                            // Include permite cargar relaciones antes de ejecutar la consulta.
                            objPreconsulta =
                                objPreconsulta.Include(x);
                        });
                }

                // CountAsync cuenta únicamente los registros que cumplen con la condición recibida.
                objRespuesta.Data =
                    await objPreconsulta.CountAsync(objPredicado);
            }
            catch (Exception ex)
            {
                // Guarda el mensaje del error si no se pudo realizar el conteo.
                objRespuesta.Error = ex.Message;

                // Deja el resultado vacío porque el conteo falló.
                objRespuesta.Data = null;
            }

            // Devuelve la cantidad obtenida o el error.
            return objRespuesta;
        }
    }
}