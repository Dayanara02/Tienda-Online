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
        // Validación básica: campos obligatorios
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
            // Validación básica: campos obligatorios
            return BadRequest(

                "La contraseña debe tener al menos 6 caracteres."

            );

        }

        var registrado =
            // Llama al servicio de autenticación para registrar al usuario
            await _authServicio.RegistrarAsync(registro);
        // Si el registro falla
        if (!registrado)

        {
            // Muestra un mensaje que ya existe o que no es valido
            return Conflict(

                "No se pudo registrar. El correo ya existe o el rol no es válido."

            );

        }

        // Si todo va bien, retorna un mensaje de éxito
        return Ok(new

        {

            mensaje = "Usuario registrado correctamente."

        });

    }

    [AllowAnonymous]// Permite el acceso sin estar autenticado

    [HttpPost("login")]

    public async Task<IActionResult> Login(LoginDto login)

    {
        // Validación básica: correo y contraseña obligatorios
        if (string.IsNullOrWhiteSpace(login.Correo) ||

            string.IsNullOrWhiteSpace(login.Contrasena))

        {

            return BadRequest(

                "El correo y la contraseña son obligatorios."

            );

        }

        var respuesta =
            // Llama al servicio de autenticación para intentar iniciar sesión
            await _authServicio.LoginAsync(login);

        // Si la respuesta es null, credenciales incorrectas
        if (respuesta == null)

        {
            //Indica que hay un error en la informacion  
            return Unauthorized(new

            {

                mensaje = "Correo o contraseña incorrectos."

            });

        }
        // Credenciales válidas: devolver los datos de sesión    
        return Ok(respuesta);

    }

}

