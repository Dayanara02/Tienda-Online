using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TiendaOnline.Utilidades;

namespace TiendaOnline.Dominio.InterfacesAD
{
    // Esta interfaz define las operaciones básicas que debe tener
    // cualquier repositorio que trabaje con una entidad de la base de datos.
    public interface IRepositorioAD<TEntity> where TEntity : class
    {
        // Inserta una nueva entidad y devuelve la entidad guardada.
        Task<Respuesta<TEntity>> InsertarAsync(TEntity objEntidad);

        // Modifica una entidad existente y devuelve la entidad actualizada.
        Task<Respuesta<TEntity>> ModificarAsync(TEntity objEntidad);

        // Elimina una entidad y devuelve true si la operación fue correcta.
        Task<Respuesta<bool>> EliminarAsync(TEntity objEntidad);

        // Obtiene todos los registros de una entidad.
        // objIncludes permite incluir relaciones con otras tablas si es necesario.
        Task<Respuesta<IEnumerable<TEntity>>> ListarAsync(
            List<string>? objIncludes = null);

        // Busca los registros que cumplan con la condición enviada.
        // objPredicado representa la condición que se aplicará en la consulta.
        Task<Respuesta<IEnumerable<TEntity>>> BuscarAsync(
            Expression<Func<TEntity, bool>> objPredicado,
            List<string>? objIncludes = null);

        // Obtiene una sola entidad que cumpla con la condición indicada.
        Task<Respuesta<TEntity>> ObtenerEntidadAsync(
            Expression<Func<TEntity, bool>> objPredicado,
            List<string>? objIncludes = null);

        // Cuenta cuántos registros cumplen con la condición enviada.
        Task<Respuesta<int?>> ContarAsync(
            Expression<Func<TEntity, bool>> objPredicado,
            List<string>? objIncludes = null);
    }
}
