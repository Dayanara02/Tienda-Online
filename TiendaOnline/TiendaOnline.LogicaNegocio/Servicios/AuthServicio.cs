using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Configuracion;
using TiendaOnline.Dominio.DTO;
using TiendaOnline.Dominio.Model;
using TiendaOnline.LogicaNegocio.Interfaces;

namespace TiendaOnline.LogicaNegocio.Servicios;

public class AuthServicio : IAuthServicio
{
    private readonly TiendaOnlineContext _context;
    private readonly IJwtServicio _jwtServicio;
    private readonly JwtConfiguracion _jwtConfiguracion;

    public AuthServicio(
        TiendaOnlineContext context,
        IJwtServicio jwtServicio,
        IOptions<JwtConfiguracion> jwtConfiguracion)
    {
        _context = context;
        _jwtServicio = jwtServicio;
        _jwtConfiguracion = jwtConfiguracion.Value;
    }

    public async Task<RespuestaLoginDto?> LoginAsync(LoginDto login)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.IdRolNavigation)
            .FirstOrDefaultAsync(u =>
                u.Correo == login.Correo &&
                u.Estado);

        if (usuario == null)
        {
            return null;
        }

        var contrasenaCorrecta = BCrypt.Net.BCrypt.Verify(
            login.Contrasena,
            usuario.Contrasena
        );

        if (!contrasenaCorrecta)
        {
            return null;
        }

        var fechaExpiracion = DateTime.UtcNow.AddMinutes(
            _jwtConfiguracion.DuracionMinutos
        );

        var nombreRol =
            usuario.IdRolNavigation?.Nombre ?? "Cliente";

        var token = _jwtServicio.GenerarToken(
            usuario,
            nombreRol,
            fechaExpiracion
        );

        return new RespuestaLoginDto
        {
            IdUsuario = usuario.IdUsuario,
            NombreCompleto =
                $"{usuario.Nombre} {usuario.Apellido}",
            Correo = usuario.Correo,
            Rol = nombreRol,
            Token = token,
            Expiracion = fechaExpiracion
        };
    }
    public async Task<bool> RegistrarAsync(
    RegistroUsuarioDto registro)
    {
        var correoNormalizado = registro.Correo
            .Trim()
            .ToLower();

        var correoExiste = await _context.Usuarios
            .AnyAsync(u => u.Correo.ToLower() == correoNormalizado);

        if (correoExiste)
        {
            return false;
        }

        var rolCliente = await _context.Rols
            .FirstOrDefaultAsync(r =>
                r.Nombre == "Cliente" &&
                r.Estado);

        if (rolCliente == null)
        {
            throw new InvalidOperationException(
                "No existe un rol Cliente activo."
            );
        }

        var usuario = new Usuario
        {
            IdRol = rolCliente.IdRol,
            Nombre = registro.Nombre.Trim(),
            Apellido = registro.Apellido.Trim(),
            Correo = correoNormalizado,

            Contrasena = BCrypt.Net.BCrypt.HashPassword(
                registro.Contrasena
            ),

            Telefono = registro.Telefono?.Trim(),
            FechaRegistro = DateTime.Now,
            Estado = true
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return true;
    }
}
