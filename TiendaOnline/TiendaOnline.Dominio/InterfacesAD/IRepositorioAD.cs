// Permite utilizar tipos básicos de C#.
using System;

// Permite trabajar con listas y colecciones.
using System.Collections.Generic;

// Permite utilizar expresiones para filtros.
using System.Linq.Expressions;

// Importa la respuesta estándar del proyecto.
using TiendaOnline.Utilidades;


namespace TiendaOnline.Dominio.InterfacesAD
{
    // Define las operaciones comunes para cualquier entidad.
    public interface IRepositorioAD<TEntity>
        where TEntity : class
    {
        // Inserta una nueva entidad.
        Task<Respuesta<TEntity>> InsertarAsync(
            TEntity objEntidad
        );

        // Modifica una entidad existente.
        Task<Respuesta<TEntity>> ModificarAsync(
            TEntity objEntidad
        );

        // Elimina una entidad.
        Task<Respuesta<bool>> EliminarAsync(
            TEntity objEntidad
        );

        // Obtiene todos los registros.
        Task<Respuesta<IEnumerable<TEntity>>> ListarAsync(
            List<string>? objIncludes = null
        );

        // Busca registros por una condición.
        Task<Respuesta<IEnumerable<TEntity>>> BuscarAsync(
            Expression<Func<TEntity, bool>> objPredicado,
            List<string>? objIncludes = null
        );

        // Obtiene un único registro.
        Task<Respuesta<TEntity>> ObtenerEntidadAsync(
            Expression<Func<TEntity, bool>> objPredicado,
            List<string>? objIncludes = null
        );

        // Cuenta los registros encontrados.
        Task<Respuesta<int?>> ContarAsync(
            Expression<Func<TEntity, bool>> objPredicado,
            List<string>? objIncludes = null
        );
    }
}