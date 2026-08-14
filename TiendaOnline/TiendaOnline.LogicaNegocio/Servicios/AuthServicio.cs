using Microsoft.EntityFrameworkCore; // Permite trabajar con Entity Framework Core.
using Microsoft.Extensions.Options; // Permite leer configuraciones mediante IOptions.
using TiendaOnline.AccesoDatos.Context; // Importa el contexto de la base de datos.
using TiendaOnline.Dominio.Configuracion; // Importa las configuraciones del proyecto.
using TiendaOnline.Dominio.DTO; // Importa los DTO utilizados.
using TiendaOnline.Dominio.Entidades; // Importa las entidades de la base de datos.
using TiendaOnline.Dominio.InterfacesLN; // Importa las interfaces de lógica de negocio.

namespace TiendaOnline.LogicaNegocio.Servicios; // Define el espacio de nombres del servicio.

// Define el servicio de autenticación.
public class AuthServicio : IAuthServicio
{
    // Guarda el contexto de la base de datos.
    private readonly TiendaOnlineContext _context;

    // Guarda el servicio encargado de crear JWT.
    private readonly IJwtServicio _jwtServicio;

    // Guarda la configuración del JWT.
    private readonly JwtConfiguracion _jwtConfiguracion;

    // Constructor del servicio.
    public AuthServicio(
        TiendaOnlineContext context, // Recibe el contexto de la base de datos.
        IJwtServicio jwtServicio, // Recibe el servicio JWT.
        IOptions<JwtConfiguracion> jwtConfiguracion) // Recibe la configuración JWT.
    {
        _context = context; // Guarda el contexto recibido.
        _jwtServicio = jwtServicio; // Guarda el servicio JWT recibido.
        _jwtConfiguracion = jwtConfiguracion.Value; // Obtiene los valores de configuración.
    }

    // Método que realiza el inicio de sesión.
    public async Task<RespuestaLoginDto?> LoginAsync(LoginDto login)
    {
        // Busca el usuario en la tabla Usuarios.
        var usuario = await _context.Usuarios
            // Carga la información del rol relacionado.
            .Include(u => u.IdRolNavigation)
            // Busca el primer usuario que cumpla las condiciones.
            .FirstOrDefaultAsync(u =>
                u.Correo == login.Correo && // Compara el correo.
                u.Estado); // Verifica que el usuario esté activo.

        // Verifica si no se encontró el usuario.
        if (usuario == null)
        {
            // Indica que el inicio de sesión falló.
            return null;
        }

        // Verifica la contraseña ingresada con la almacenada.
        var contrasenaCorrecta = BCrypt.Net.BCrypt.Verify(
            login.Contrasena, // Contraseña ingresada.
            usuario.Contrasena // Contraseña almacenada.
        );

        // Verifica si la contraseña es incorrecta.
        if (!contrasenaCorrecta)
        {
            // Indica que el inicio de sesión falló.
            return null;
        }

        // Calcula la fecha y hora de vencimiento del token.
        var fechaExpiracion = DateTime.UtcNow.AddMinutes(
            _jwtConfiguracion.DuracionMinutos // Agrega los minutos configurados.
        );

        // Obtiene el nombre del rol del usuario.
        var nombreRol =
            usuario.IdRolNavigation?.Nombre ?? "Cliente"; // Usa Cliente si no existe rol.

        // Genera el token JWT.
        var token = _jwtServicio.GenerarToken(
            usuario, // Envía los datos del usuario.
            nombreRol, // Envía el nombre del rol.
            fechaExpiracion // Envía la fecha de vencimiento.
        );

        // Devuelve la información del inicio de sesión.
        return new RespuestaLoginDto
        {
            IdUsuario = usuario.IdUsuario, // Guarda el ID del usuario.
            NombreCompleto =
                $"{usuario.Nombre} {usuario.Apellido}", // Une nombre y apellido.
            Correo = usuario.Correo, // Guarda el correo.
            Rol = nombreRol, // Guarda el rol.
            Token = token, // Guarda el token generado.
            Expiracion = fechaExpiracion // Guarda la fecha de expiración.
        };
    }

    // Método que registra un nuevo usuario.
    public async Task<bool> RegistrarAsync(
    RegistroUsuarioDto registro)
    {
        // Normaliza el correo recibido.
        var correoNormalizado = registro.Correo
            .Trim() // Elimina espacios al inicio y final.
            .ToLower(); // Convierte el correo a minúsculas.

        // Comprueba si el correo ya existe.
        var correoExiste = await _context.Usuarios
            .AnyAsync(u => u.Correo.ToLower() == correoNormalizado); // Compara los correos.

        // Verifica si el correo ya está registrado.
        if (correoExiste)
        {
            // Indica que el registro no puede realizarse.
            return false;
        }

        // Busca el rol Cliente activo.
        var rolCliente = await _context.Rols
            .FirstOrDefaultAsync(r =>
                r.Nombre == "Cliente" && // Busca el rol Cliente.
                r.Estado); // Verifica que esté activo.

        // Verifica si el rol Cliente no existe.
        if (rolCliente == null)
        {
            // Genera un error indicando que falta el rol.
            throw new InvalidOperationException(
                "No existe un rol Cliente activo."
            );
        }

        // Crea un nuevo objeto Usuario.
        var usuario = new Usuario
        {
            IdRol = rolCliente.IdRol, // Asigna el ID del rol.
            Nombre = registro.Nombre.Trim(), // Guarda el nombre sin espacios.
            Apellido = registro.Apellido.Trim(), // Guarda el apellido sin espacios.
            Correo = correoNormalizado, // Guarda el correo normalizado.

            // Encripta la contraseña antes de guardarla.
            Contrasena = BCrypt.Net.BCrypt.HashPassword(
                registro.Contrasena // Recibe la contraseña del registro.
            ),

            Telefono = registro.Telefono?.Trim(), // Guarda el teléfono sin espacios.
            FechaRegistro = DateTime.Now, // Guarda la fecha actual.
            Estado = true // Activa el usuario.
        };

        _context.Usuarios.Add(usuario); // Agrega el usuario al contexto.
        await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos.

        return true; // Indica que el registro fue exitoso.
    }
}