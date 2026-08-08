using Microsoft.AspNetCore.Mvc;
using TiendaOnline.Dominio.DTO;
using TiendaOnline.Dominio.InterfacesLN;
using Microsoft.AspNetCore.Authorization;

namespace TiendaOnline.API.Controllers;

[ApiController]

[Route("api/[controller]")]

public class AuthController : ControllerBase

{

    private readonly IAuthServicio _authServicio;

    public AuthController(IAuthServicio authServicio)

    {

        _authServicio = authServicio;

    }

    [AllowAnonymous]

    [HttpPost("registrar")]

    public async Task<IActionResult> Registrar(

        RegistroUsuarioDto registro)

    {

        if (string.IsNullOrWhiteSpace(registro.Nombre) ||

            string.IsNullOrWhiteSpace(registro.Apellido) ||

            string.IsNullOrWhiteSpace(registro.Correo) ||

            string.IsNullOrWhiteSpace(registro.Contrasena))

        {

            return BadRequest(

                "Nombre, apellido, correo y contraseña son obligatorios."

            );

        }

        if (registro.Contrasena.Length < 6)

        {

            return BadRequest(

                "La contraseña debe tener al menos 6 caracteres."

            );

        }

        var registrado =

            await _authServicio.RegistrarAsync(registro);

        if (!registrado)

        {

            return Conflict(

                "No se pudo registrar. El correo ya existe o el rol no es válido."

            );

        }

        return Ok(new

        {

            mensaje = "Usuario registrado correctamente."

        });

    }

    [AllowAnonymous]

    [HttpPost("login")]

    public async Task<IActionResult> Login(LoginDto login)

    {

        if (string.IsNullOrWhiteSpace(login.Correo) ||

            string.IsNullOrWhiteSpace(login.Contrasena))

        {

            return BadRequest(

                "El correo y la contraseña son obligatorios."

            );

        }

        var respuesta =

            await _authServicio.LoginAsync(login);

        if (respuesta == null)

        {

            return Unauthorized(new

            {

                mensaje = "Correo o contraseña incorrectos."

            });

        }

        return Ok(respuesta);

    }

}
