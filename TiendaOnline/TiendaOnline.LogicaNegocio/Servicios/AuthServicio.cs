using Microsoft.EntityFrameworkCore; // Permite trabajar con Entity Framework Core.
using Microsoft.Extensions.Options; // Permite leer configuraciones mediante IOptions.
using TiendaOnline.AccesoDatos.Context; // Importa el contexto de la base de datos.
using TiendaOnline.Dominio.Configuracion; // Importa las configuraciones del proyecto.
using TiendaOnline.Dominio.DTO; // Importa los DTO utilizados.
using TiendaOnline.Dominio.Entidades; // Importa las entidades de la base de datos.
using TiendaOnline.Dominio.InterfacesLN; // Importa las interfaces de lógica de negocio.

namespace TiendaOnline.LogicaNegocio.Servicios; // Define el espacio de nombres del servicio.


// =====================================================
// SERVICIO DE AUTENTICACIÓN
// =====================================================

// Define el servicio encargado
// del inicio de sesión y registro.
public class AuthServicio : IAuthServicio
{
    // Guarda el contexto
    // de la base de datos.
    private readonly TiendaOnlineContext _context;

    // Guarda el servicio encargado
    // de generar los tokens JWT.
    private readonly IJwtServicio _jwtServicio;

    // Guarda la configuración
    // utilizada por JWT.
    private readonly JwtConfiguracion _jwtConfiguracion;


    // =====================================================
    // CONSTRUCTOR
    // =====================================================

    // Recibe las dependencias necesarias
    // para trabajar con usuarios y JWT.
    public AuthServicio(
        TiendaOnlineContext context,
        IJwtServicio jwtServicio,
        IOptions<JwtConfiguracion> jwtConfiguracion)
    {
        // Guarda el contexto recibido.
        _context = context;

        // Guarda el servicio JWT.
        _jwtServicio = jwtServicio;

        // Obtiene los valores
        // de configuración del JWT.
        _jwtConfiguracion = jwtConfiguracion.Value;
    }


    // =====================================================
    // INICIAR SESIÓN
    // =====================================================

    // Comprueba correo y contraseña
    // y devuelve los datos del usuario.
    public async Task<RespuestaLoginDto?> LoginAsync(
        LoginDto login)
    {
        // Busca al usuario por correo
        // y comprueba que esté activo.
        var usuario = await _context.Usuarios

            // También carga el rol
            // relacionado con el usuario.
            .Include(
                u => u.IdRolNavigation
            )

            // Busca el primer usuario
            // que cumpla las condiciones.
            .FirstOrDefaultAsync(
                u =>
                    u.Correo == login.Correo &&
                    u.Estado
            );


        // Si el usuario no existe,
        // el inicio de sesión falla.
        if (usuario == null)
        {
            return null;
        }


        // Comprueba la contraseña ingresada
        // contra la contraseña cifrada en SQL.
        var contrasenaCorrecta =
            BCrypt.Net.BCrypt.Verify(
                login.Contrasena,
                usuario.Contrasena
            );


        // Si la contraseña es incorrecta,
        // no permite iniciar sesión.
        if (!contrasenaCorrecta)
        {
            return null;
        }


        // Calcula cuándo debe
        // expirar el token JWT.
        var fechaExpiracion =
            DateTime.UtcNow.AddMinutes(
                _jwtConfiguracion.DuracionMinutos
            );


        // Obtiene el nombre
        // del rol del usuario.
        var nombreRol =
            usuario.IdRolNavigation?.Nombre
            ?? "Cliente";


        // Genera el token JWT
        // utilizando el usuario real.
        var token =
            _jwtServicio.GenerarToken(
                usuario,
                nombreRol,
                fechaExpiracion
            );


        // Devuelve todos los datos
        // necesarios después del login.
        return new RespuestaLoginDto
        {
            // ID real del usuario
            // registrado en SQL Server.
            IdUsuario =
                usuario.IdUsuario,

            // Nombre completo.
            NombreCompleto =
                $"{usuario.Nombre} {usuario.Apellido}",

            // Correo.
            Correo =
                usuario.Correo,

            // Rol.
            Rol =
                nombreRol,

            // JWT generado.
            Token =
                token,

            // Fecha de vencimiento.
            Expiracion =
                fechaExpiracion
        };
    }


    // =====================================================
    // REGISTRAR USUARIO
    // =====================================================

    // Registra un nuevo usuario
    // dentro de SQL Server.
    public async Task<bool> RegistrarAsync(
        RegistroUsuarioDto registro)
    {
        // Normaliza el correo.
        var correoNormalizado =
            registro.Correo
                .Trim()
                .ToLower();


        // Comprueba si ya existe
        // otro usuario con ese correo.
        var correoExiste =
            await _context.Usuarios
                .AnyAsync(
                    u =>
                        u.Correo.ToLower()
                        ==
                        correoNormalizado
                );


        // Si el correo ya existe,
        // cancela el registro.
        if (correoExiste)
        {
            return false;
        }


        // Busca el rol Cliente
        // dentro de la base de datos.
        var rolCliente =
            await _context.Rols
                .FirstOrDefaultAsync(
                    r =>
                        r.Nombre == "Cliente"
                        &&
                        r.Estado
                );


        // Si no existe el rol Cliente,
        // genera una excepción.
        if (rolCliente == null)
        {
            throw new InvalidOperationException(
                "No existe un rol Cliente activo."
            );
        }


        // Crea el nuevo usuario.
        var usuario =
            new Usuario
            {
                // Asigna el rol Cliente.
                IdRol =
                    rolCliente.IdRol,

                // Guarda el nombre.
                Nombre =
                    registro.Nombre.Trim(),

                // Guarda el apellido.
                Apellido =
                    registro.Apellido.Trim(),

                // Guarda el correo normalizado.
                Correo =
                    correoNormalizado,

                // Cifra la contraseña.
                Contrasena =
                    BCrypt.Net.BCrypt.HashPassword(
                        registro.Contrasena
                    ),

                // Guarda el teléfono.
                Telefono =
                    registro.Telefono?.Trim(),

                // Guarda la fecha actual.
                FechaRegistro =
                    DateTime.Now,

                // Deja activo al usuario.
                Estado =
                    true
            };


        // Agrega el usuario
        // al contexto.
        _context.Usuarios.Add(
            usuario
        );


        // Guarda el usuario
        // físicamente en SQL Server.
        await _context.SaveChangesAsync();


        // Registro exitoso.
        return true;
    }
}