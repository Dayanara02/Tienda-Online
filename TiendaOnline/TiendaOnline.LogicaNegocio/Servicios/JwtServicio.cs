using Microsoft.Extensions.Options; // Permite leer la configuración mediante IOptions.
using Microsoft.IdentityModel.Tokens; // Permite trabajar con claves y seguridad del token.
using System.IdentityModel.Tokens.Jwt; // Permite crear y manejar tokens JWT.
using System.Security.Claims; // Permite crear información dentro del token.
using System.Text; // Permite trabajar con codificación de texto.
using TiendaOnline.Dominio.Configuracion; // Importa la configuración del JWT.
using TiendaOnline.Dominio.Entidades; // Importa la entidad Usuario.
using TiendaOnline.Dominio.InterfacesLN; // Importa la interfaz del servicio JWT.

namespace TiendaOnline.LogicaNegocio.Servicios; // Define el espacio de nombres del servicio.

// Define el servicio encargado de generar tokens JWT.
public class JwtServicio : IJwtServicio
{
    // Guarda la configuración del JWT.
    private readonly JwtConfiguracion _configuracion;

    // Constructor del servicio.
    public JwtServicio(
        IOptions<JwtConfiguracion> configuracion) // Recibe la configuración JWT.
    {
        _configuracion = configuracion.Value; // Obtiene los valores de configuración.
    }

    // Método que genera un token para el usuario.
    public string GenerarToken(
        Usuario usuario, // Recibe los datos del usuario.
        string nombreRol, // Recibe el nombre del rol.
        DateTime fechaExpiracion) // Recibe la fecha de vencimiento.
    {
        // Crea la lista de datos que tendrá el token.
        var claims = new List<Claim>
        {
            // Guarda el ID del usuario.
            new Claim(
                ClaimTypes.NameIdentifier, // Identifica al usuario.
                usuario.IdUsuario.ToString()), // Convierte el ID a texto.

            // Guarda el nombre completo del usuario.
            new Claim(
                ClaimTypes.Name, // Define el nombre del usuario.
                $"{usuario.Nombre} {usuario.Apellido}"), // Une nombre y apellido.

            // Guarda el correo del usuario.
            new Claim(
                ClaimTypes.Email, // Define el correo.
                usuario.Correo), // Asigna el correo.

            // Guarda el rol del usuario.
            new Claim(
                ClaimTypes.Role, // Define el tipo de rol.
                nombreRol), // Asigna el nombre del rol.

            // Genera un identificador único para el token.
            new Claim(
                JwtRegisteredClaimNames.Jti, // Identifica de forma única el token.
                Guid.NewGuid().ToString()) // Genera un GUID nuevo.
        };

        // Convierte la clave secreta a bytes.
        var claveBytes = Encoding.UTF8.GetBytes(
            _configuracion.Clave); // Obtiene la clave configurada.

        // Crea la clave de seguridad simétrica.
        var claveSeguridad =
            new SymmetricSecurityKey(claveBytes); // Usa los bytes de la clave.

        // Crea las credenciales para firmar el token.
        var credenciales = new SigningCredentials(
            claveSeguridad, // Usa la clave de seguridad.
            SecurityAlgorithms.HmacSha256); // Define el algoritmo de firma.

        // Crea el token JWT.
        var token = new JwtSecurityToken(
            issuer: _configuracion.Emisor, // Define quién emite el token.
            audience: _configuracion.Audiencia, // Define para quién es el token.
            claims: claims, // Agrega los datos del usuario.
            expires: fechaExpiracion, // Define cuándo expira.
            signingCredentials: credenciales // Agrega la firma de seguridad.
        );

        // Convierte el token en una cadena.
        return new JwtSecurityTokenHandler()
            .WriteToken(token); // Genera el JWT final.
    }
}