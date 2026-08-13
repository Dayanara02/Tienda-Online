using Microsoft.Extensions.Logging;
using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Dominio.InterfacesAD;
using TiendaOnline.Dominio.InterfacesLN;
using TiendaOnline.Utilidades;

namespace TiendaOnline.LogicaNegocio.Implementaciones
{
    // Esta clase contiene las reglas de negocio relacionadas con los usuarios.
    // Implementa IUsuarioLN para cumplir con las operaciones definidas
    // para registrar, consultar, modificar, eliminar y buscar usuarios.
    public class UsuarioLN : IUsuarioLN
    {
        // Guarda la unidad de trabajo.
        // Por medio de ella se obtiene el repositorio de usuarios
        // sin acceder directamente al contexto de Entity Framework.
        private readonly IUnidadTrabajoEF _unidadDeTrabajo;

        // Permite registrar errores que ocurran dentro
        // de la lógica de negocio de usuarios.
        private readonly ILogger<UsuarioLN> _logger;


        // El constructor recibe las dependencias necesarias.
        // Estas pueden ser entregadas mediante inyección de dependencias.
        public UsuarioLN(
            IUnidadTrabajoEF unidadDeTrabajo,
            ILogger<UsuarioLN> logger)
        {
            // Guarda la unidad de trabajo para poder utilizar TUsuario.
            _unidadDeTrabajo = unidadDeTrabajo;

            // Guarda el logger para registrar errores.
            _logger = logger;
        }


        // Este método registra un nuevo usuario en el sistema.
        public async Task<Respuesta<Usuario>> InsertarAsync(Usuario datos)
        {
            // Crea el objeto que devolverá el usuario registrado
            // o el mensaje de error si ocurre algún problema.
            var resultado = new Respuesta<Usuario>();

            try
            {
                // Valida que el nombre tenga información.
                if (string.IsNullOrWhiteSpace(datos.Nombre))
                {
                    resultado.Error =
                        "El nombre del usuario es obligatorio.";

                    return resultado;
                }


                // Valida que el apellido tenga información.
                if (string.IsNullOrWhiteSpace(datos.Apellido))
                {
                    resultado.Error =
                        "El apellido del usuario es obligatorio.";

                    return resultado;
                }


                // Valida que el correo electrónico tenga información.
                if (string.IsNullOrWhiteSpace(datos.Correo))
                {
                    resultado.Error =
                        "El correo electrónico es obligatorio.";

                    return resultado;
                }


                // Busca si ya existe otro usuario registrado
                // con exactamente el mismo correo electrónico.
                var usuarioExistente =
                    await _unidadDeTrabajo.TUsuario.ObtenerEntidadAsync(
                        x => x.Correo == datos.Correo);

                // Si encuentra un usuario, evita registrar
                // dos cuentas con el mismo correo.
                if (usuarioExistente.Data != null)
                {
                    // Guarda el usuario existente dentro de la respuesta.
                    resultado.Data = usuarioExistente.Data;

                    // Explica por qué no se puede realizar el registro.
                    resultado.Error =
                        "Ya existe un usuario registrado con ese correo.";

                    // Termina el método sin insertar un nuevo registro.
                    return resultado;
                }


                // Valida que se haya indicado un rol válido.
                if (datos.IdRol <= 0)
                {
                    resultado.Error =
                        "Debe indicar un rol válido para el usuario.";

                    return resultado;
                }


                // Valida que la contraseña tenga información.
                if (string.IsNullOrWhiteSpace(datos.Contrasena))
                {
                    resultado.Error =
                        "La contraseña es obligatoria.";

                    return resultado;
                }


                // Guarda automáticamente la fecha
                // en la que se registra el usuario.
                datos.FechaRegistro = DateTime.Now;


                // Envía el usuario al repositorio
                // para guardarlo en la base de datos.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TUsuario.InsertarAsync(datos);


                // Comprueba si el repositorio devolvió algún error.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    // Copia el error para devolverlo a la capa superior.
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda en la respuesta el usuario registrado.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra técnicamente el error
                // indicando qué correo se intentaba registrar.
                _logger.LogError(
                    ex,
                    "Error al insertar el usuario con correo {Correo}",
                    datos.Correo);

                // Guarda el mensaje del error dentro de la respuesta.
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado final de la operación.
            return resultado;
        }


        // Este método obtiene todos los usuarios registrados.
        public async Task<Respuesta<IEnumerable<Usuario>>> ListarAsync()
        {
            // Crea una respuesta capaz de devolver varios usuarios.
            var resultado =
                new Respuesta<IEnumerable<Usuario>>();

            try
            {
                // Solicita al repositorio todos los usuarios.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TUsuario.ListarAsync();


                // Comprueba si ocurrió algún error durante la consulta.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda la lista de usuarios obtenida.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra cualquier error ocurrido
                // durante la consulta de usuarios.
                _logger.LogError(
                    ex,
                    "Error al listar los usuarios.");

                // Guarda el mensaje del error.
                resultado.Error = ex.Message;
            }

            // Devuelve la lista de usuarios o el error.
            return resultado;
        }


        // Este método modifica los datos de un usuario existente.
        public async Task<Respuesta<Usuario>> ModificarAsync(Usuario datos)
        {
            // Crea la respuesta donde se devolverá
            // el usuario modificado.
            var resultado = new Respuesta<Usuario>();

            try
            {
                // Busca primero el usuario utilizando su IdUsuario.
                var usuarioActual =
                    await _unidadDeTrabajo.TUsuario.ObtenerEntidadAsync(
                        x => x.IdUsuario == datos.IdUsuario);


                // Si el usuario no existe,
                // no se puede realizar la modificación.
                if (usuarioActual.Data == null)
                {
                    resultado.Error =
                        "No existe el usuario que desea modificar.";

                    return resultado;
                }


                // Comprueba que el nombre siga teniendo información válida.
                if (string.IsNullOrWhiteSpace(datos.Nombre))
                {
                    resultado.Error =
                        "El nombre del usuario es obligatorio.";

                    return resultado;
                }


                // Comprueba que el apellido siga teniendo información válida.
                if (string.IsNullOrWhiteSpace(datos.Apellido))
                {
                    resultado.Error =
                        "El apellido del usuario es obligatorio.";

                    return resultado;
                }


                // Comprueba que el correo no esté vacío.
                if (string.IsNullOrWhiteSpace(datos.Correo))
                {
                    resultado.Error =
                        "El correo electrónico es obligatorio.";

                    return resultado;
                }


                // Verifica que no exista otro usuario diferente
                // utilizando el mismo correo electrónico.
                var correoExistente =
                    await _unidadDeTrabajo.TUsuario.ObtenerEntidadAsync(
                        x => x.Correo == datos.Correo &&
                             x.IdUsuario != datos.IdUsuario);


                // Si encuentra otro usuario con el mismo correo,
                // evita guardar información duplicada.
                if (correoExistente.Data != null)
                {
                    resultado.Error =
                        "Ya existe otro usuario registrado con ese correo.";

                    return resultado;
                }


                // Valida que el rol recibido sea válido.
                if (datos.IdRol <= 0)
                {
                    resultado.Error =
                        "El rol del usuario no es válido.";

                    return resultado;
                }


                // Actualiza el rol del usuario.
                usuarioActual.Data.IdRol = datos.IdRol;

                // Actualiza el nombre.
                usuarioActual.Data.Nombre = datos.Nombre;

                // Actualiza el apellido.
                usuarioActual.Data.Apellido = datos.Apellido;

                // Actualiza el correo electrónico.
                usuarioActual.Data.Correo = datos.Correo;

                // Actualiza el teléfono.
                usuarioActual.Data.Telefono = datos.Telefono;

                // Actualiza si la cuenta está activa o inactiva.
                usuarioActual.Data.Estado = datos.Estado;


                // Solo actualiza la contraseña
                // si realmente se recibió una nueva.
                if (!string.IsNullOrWhiteSpace(datos.Contrasena))
                {
                    // Sustituye la contraseña anterior
                    // por el nuevo valor recibido.
                    usuarioActual.Data.Contrasena = datos.Contrasena;
                }


                // Envía el usuario actualizado al repositorio
                // para guardar los cambios en la base de datos.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TUsuario.ModificarAsync(
                        usuarioActual.Data);


                // Comprueba si el repositorio devolvió algún error.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda el usuario modificado dentro de la respuesta.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el identificador del usuario
                // que se intentaba modificar.
                _logger.LogError(
                    ex,
                    "Error al modificar el usuario con IdUsuario {IdUsuario}",
                    datos.IdUsuario);

                // Guarda el mensaje del error.
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado de la modificación.
            return resultado;
        }


        // Este método elimina un usuario existente.
        public async Task<Respuesta<bool>> EliminarAsync(Usuario datos)
        {
            // Crea una respuesta booleana.
            // True indica que se eliminó y false que no se pudo eliminar.
            var resultado = new Respuesta<bool>();

            try
            {
                // Busca primero el usuario utilizando su identificador.
                var usuario =
                    await _unidadDeTrabajo.TUsuario.ObtenerEntidadAsync(
                        x => x.IdUsuario == datos.IdUsuario);


                // Comprueba que el usuario realmente exista.
                if (usuario.Data == null)
                {
                    // Indica que la eliminación no se realizó.
                    resultado.Data = false;

                    // Explica por qué no se pudo eliminar.
                    resultado.Error =
                        "No existe el usuario que desea eliminar.";

                    return resultado;
                }


                // Envía el usuario encontrado al repositorio
                // para eliminarlo de la base de datos.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TUsuario.EliminarAsync(
                        usuario.Data);


                // Comprueba si ocurrió algún error durante la eliminación.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Data = false;
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda el resultado de la eliminación.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra qué usuario produjo el error.
                _logger.LogError(
                    ex,
                    "Error al eliminar el usuario con IdUsuario {IdUsuario}",
                    datos.IdUsuario);

                // Indica que la operación no fue completada.
                resultado.Data = false;

                // Guarda el mensaje del error.
                resultado.Error = ex.Message;
            }

            // Devuelve el resultado de la eliminación.
            return resultado;
        }


        // Este método busca usuarios utilizando
        // el nombre recibido como criterio.
        public async Task<Respuesta<IEnumerable<Usuario>>> BuscarAsync(
            Usuario datos)
        {
            // Crea una respuesta capaz de devolver varios usuarios.
            var resultado =
                new Respuesta<IEnumerable<Usuario>>();

            try
            {
                // Si el nombre llega nulo,
                // utiliza una cadena vacía para evitar errores.
                var nombre = datos.Nombre ?? string.Empty;


                // Busca todos los usuarios cuyo nombre
                // contenga el texto recibido.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TUsuario.BuscarAsync(
                        x => x.Nombre.Contains(nombre));


                // Comprueba si ocurrió algún error en la búsqueda.
                if (!string.IsNullOrEmpty(respuestaRepositorio.Error))
                {
                    resultado.Error = respuestaRepositorio.Error;

                    return resultado;
                }


                // Guarda los usuarios encontrados.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra el error ocurrido durante la búsqueda.
                _logger.LogError(
                    ex,
                    "Error al buscar usuarios.");

                // Guarda el mensaje del error.
                resultado.Error = ex.Message;
            }

            // Devuelve los usuarios encontrados o el error.
            return resultado;
        }


        // Este método obtiene un usuario específico
        // utilizando su IdUsuario.
        public async Task<Respuesta<Usuario>> ObtenerAsync(Usuario datos)
        {
            // Crea una respuesta para devolver un solo usuario.
            var resultado = new Respuesta<Usuario>();

            try
            {
                // Busca el usuario cuyo IdUsuario
                // coincida con el identificador recibido.
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TUsuario.ObtenerEntidadAsync(
                        x => x.IdUsuario == datos.IdUsuario);


                // Comprueba si el usuario fue encontrado.
                if (respuestaRepositorio.Data == null)
                {
                    resultado.Error =
                        "Usuario no encontrado.";

                    return resultado;
                }


                // Guarda el usuario encontrado dentro de la respuesta.
                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                // Registra qué IdUsuario produjo el error.
                _logger.LogError(
                    ex,
                    "Error al obtener el usuario con IdUsuario {IdUsuario}",
                    datos.IdUsuario);

                // Guarda el mensaje del error.
                resultado.Error = ex.Message;
            }

            // Devuelve el usuario encontrado o el error.
            return resultado;
        }
    }
}