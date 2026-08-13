// Permite crear controladores y respuestas HTTP.
using Microsoft.AspNetCore.Mvc;

// Importa los DTO utilizados en autenticación.
using TiendaOnline.Dominio.DTO;

// Importa la interfaz del servicio de autenticación.
using TiendaOnline.Dominio.InterfacesLN;

// Permite utilizar atributos de autorización.
using Microsoft.AspNetCore.Authorization;

namespace TiendaOnline.API.Controllers;

// Indica que esta clase funciona como controlador de API.
[ApiController]

// Define la ruta principal del controlador.
// En este caso será: api/Auth
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // Guarda el servicio de autenticación.
    private readonly IAuthServicio _authServicio;


    // Recibe el servicio por inyección de dependencias.
    public AuthController(
        IAuthServicio authServicio
    )
    {
        // Guarda el servicio recibido.
        _authServicio = authServicio;
    }


    // REGISTRO
    // Permite registrar sin iniciar sesión.
    [AllowAnonymous]

    // Define la ruta POST api/Auth/registrar.
    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar(
        RegistroUsuarioDto registro
    )
    {
        // Valida los campos obligatorios.
        if (
            string.IsNullOrWhiteSpace(registro.Nombre) ||
            string.IsNullOrWhiteSpace(registro.Apellido) ||
            string.IsNullOrWhiteSpace(registro.Correo) ||
            string.IsNullOrWhiteSpace(registro.Contrasena)
        )
        {
            return BadRequest(
                "Nombre, apellido, correo y contraseña son obligatorios."
            );
        }


        // Valida la longitud mínima de la contraseña.
        if (registro.Contrasena.Length < 6)
        {
            return BadRequest(
                "La contraseña debe tener al menos 6 caracteres."
            );
        }


        // Intenta registrar el usuario.
        var registrado =
            await _authServicio.RegistrarAsync(
                registro
            );


        // Verifica si el registro falló.
        if (!registrado)
        {
            return Conflict(
                "No se pudo registrar. " +
                "El correo ya existe o el rol no es válido."
            );
        }


        // Devuelve una respuesta correcta.
        return Ok(
            new
            {
                mensaje =
                    "Usuario registrado correctamente."
            }
        );
    }



    // LOGIN
    // Permite iniciar sesión sin tener token.
    [AllowAnonymous]

    // Define la ruta POST api/Auth/login.
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginDto login
    )
    {
        // Valida correo y contraseña.
        if (
            string.IsNullOrWhiteSpace(login.Correo) ||
            string.IsNullOrWhiteSpace(login.Contrasena)
        )
        {
            return BadRequest(
                "El correo y la contraseña son obligatorios."
            );
        }


        // Intenta iniciar sesión.
        var respuesta =
            await _authServicio.LoginAsync(
                login
            );


        // Comprueba si las credenciales son incorrectas.
        if (respuesta == null)
        {
            return Unauthorized(
                new
                {
                    mensaje =
                        "Correo o contraseña incorrectos."
                }
            );
        }


        // Devuelve token y datos del usuario.
        return Ok(
            respuesta
        );
    }
}