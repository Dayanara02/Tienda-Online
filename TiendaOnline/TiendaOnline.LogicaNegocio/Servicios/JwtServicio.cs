using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TiendaOnline.Dominio.Configuracion;
using TiendaOnline.Dominio.Entidades;
using TiendaOnline.Dominio.InterfacesLN;

namespace TiendaOnline.LogicaNegocio.Servicios;

public class JwtServicio : IJwtServicio
{
    private readonly JwtConfiguracion _configuracion;

    public JwtServicio(
        IOptions<JwtConfiguracion> configuracion)
    {
        _configuracion = configuracion.Value;
    }

    public string GenerarToken(
        Usuario usuario,
        string nombreRol,
        DateTime fechaExpiracion)
    {
        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                usuario.IdUsuario.ToString()),

            new Claim(
                ClaimTypes.Name,
                $"{usuario.Nombre} {usuario.Apellido}"),

            new Claim(
                ClaimTypes.Email,
                usuario.Correo),

            new Claim(
                ClaimTypes.Role,
                nombreRol),

            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        var claveBytes = Encoding.UTF8.GetBytes(
            _configuracion.Clave);

        var claveSeguridad =
            new SymmetricSecurityKey(claveBytes);

        var credenciales = new SigningCredentials(
            claveSeguridad,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuracion.Emisor,
            audience: _configuracion.Audiencia,
            claims: claims,
            expires: fechaExpiracion,
            signingCredentials: credenciales
        );

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}