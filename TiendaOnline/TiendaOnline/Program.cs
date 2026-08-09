// Permite configurar la autenticación mediante tokens JWT.
using Microsoft.AspNetCore.Authentication.JwtBearer;

// Permite trabajar con Entity Framework Core
// y configurar la conexión con SQL Server.
using Microsoft.EntityFrameworkCore;

// Permite validar tokens JWT,
// incluyendo firma, emisor, audiencia y expiración.
using Microsoft.IdentityModel.Tokens;

// Permite configurar Swagger.
using Microsoft.OpenApi;

// Permite convertir la clave JWT de texto
// a un arreglo de bytes.
using System.Text;

// Permite configurar cómo se serializa JSON
// y evitar problemas con relaciones circulares.
using System.Text.Json.Serialization;

// Importa las interfaces de lógica de negocio.
using TiendaOnline.Dominio.InterfacesLN;

// Importa las implementaciones de servicios
// como JwtServicio, AuthServicio y PedidoServicio.
using TiendaOnline.LogicaNegocio.Servicios;

// Importa el contexto de la base de datos.
using TiendaOnline.AccesoDatos.Context;

// Importa la clase que representa
// la configuración JWT del appsettings.json.
using TiendaOnline.Dominio.Configuracion;

// Importa el manejador global de excepciones.
using TiendaOnline.API.Manejadores;


// Define la clase principal del programa.
public partial class Program
{
    // Método principal donde se configura
    // y se inicia la API.
    private static void Main(string[] args)
    {
        // Crea el constructor principal de la aplicación.
        var builder =
            WebApplication.CreateBuilder(args);


        // =========================================================
        // MANEJO GLOBAL DE EXCEPCIONES
        // =========================================================

        // Registra el manejador personalizado de excepciones.
        builder.Services
            .AddExceptionHandler<ManejadorGlobalExcepciones>();

        // Permite utilizar respuestas estándar
        // para errores de la API.
        builder.Services
            .AddProblemDetails();


        // =========================================================
        // CONFIGURACIÓN JWT
        // =========================================================

        // Lee la sección "Jwt" del archivo appsettings.json
        // y la conecta con la clase JwtConfiguracion.
        builder.Services.Configure<JwtConfiguracion>(
            builder.Configuration.GetSection("Jwt")
        );


        // =========================================================
        // SERVICIOS DE LÓGICA DE NEGOCIO
        // =========================================================

        // Registra el servicio que genera tokens JWT.
        builder.Services.AddScoped<
            IJwtServicio,
            JwtServicio
        >();

        // Registra el servicio encargado
        // del login y registro de usuarios.
        builder.Services.AddScoped<
            IAuthServicio,
            AuthServicio
        >();

        // Registra el servicio encargado
        // de crear y confirmar pedidos.
        builder.Services.AddScoped<
            IPedidoServicio,
            PedidoServicio
        >();


        // Obtiene directamente la configuración JWT
        // para utilizarla en la validación de tokens.
        var jwtConfiguracion =
            builder.Configuration
                .GetSection("Jwt")
                .Get<JwtConfiguracion>();


        // Comprueba que exista la configuración JWT
        // y que la clave no esté vacía.
        if (
            jwtConfiguracion == null ||
            string.IsNullOrWhiteSpace(
                jwtConfiguracion.Clave
            )
        )
        {
            // Detiene la aplicación si falta
            // una configuración necesaria.
            throw new InvalidOperationException(
                "La configuración JWT no está completa."
            );
        }


        // Convierte la clave secreta
        // desde texto hacia bytes.
        var clave =
            Encoding.UTF8.GetBytes(
                jwtConfiguracion.Clave
            );


        // =========================================================
        // AUTENTICACIÓN JWT
        // =========================================================

        // Configura JWT como el sistema principal
        // de autenticación de la API.
        builder.Services
            .AddAuthentication(options =>
            {
                // Indica qué esquema se utiliza
                // para autenticar automáticamente.
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults
                        .AuthenticationScheme;

                // Indica qué esquema se utiliza
                // cuando un usuario no está autorizado.
                options.DefaultChallengeScheme =
                    JwtBearerDefaults
                        .AuthenticationScheme;
            })

            // Agrega la configuración específica
            // para tokens Bearer.
            .AddJwtBearer(options =>
            {
                // Permite conservar el token.
                options.SaveToken = true;

                // Durante desarrollo permite
                // trabajar sin exigir metadata HTTPS.
                options.RequireHttpsMetadata = false;


                // Define todas las reglas
                // que debe cumplir un token.
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        // Comprueba que el emisor
                        // del token sea correcto.
                        ValidateIssuer = true,

                        // Comprueba que la audiencia
                        // del token sea correcta.
                        ValidateAudience = true,

                        // Comprueba que el token
                        // todavía no haya expirado.
                        ValidateLifetime = true,

                        // Comprueba que la firma
                        // del token sea válida.
                        ValidateIssuerSigningKey = true,

                        // Emisor esperado.
                        ValidIssuer =
                            jwtConfiguracion.Emisor,

                        // Audiencia esperada.
                        ValidAudience =
                            jwtConfiguracion.Audiencia,

                        // Clave que se utiliza
                        // para comprobar la firma del token.
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                clave
                            ),

                        // No agrega minutos adicionales
                        // de tolerancia a la expiración.
                        ClockSkew =
                            TimeSpan.Zero
                    };


                // =================================================
                // DIAGNÓSTICO TEMPORAL DEL TOKEN
                // =================================================

                // Estos eventos nos permitirán ver
                // en la consola de Visual Studio
                // por qué exactamente falla la autenticación.
                options.Events =
                    new JwtBearerEvents
                    {
                        // Este evento se ejecuta
                        // cuando la validación del token falla.
                        OnAuthenticationFailed =
                            context =>
                            {
                                // Imprime un título visible
                                // para encontrar fácilmente el error.
                                Console.WriteLine(
                                    "=================================="
                                );

                                Console.WriteLine(
                                    "ERROR JWT"
                                );

                                // Muestra el tipo exacto
                                // de excepción que ocurrió.
                                Console.WriteLine(
                                    "Tipo: " +
                                    context.Exception
                                        .GetType()
                                        .Name
                                );

                                // Muestra el mensaje exacto
                                // de la excepción.
                                Console.WriteLine(
                                    "Mensaje: " +
                                    context.Exception
                                        .Message
                                );

                                Console.WriteLine(
                                    "=================================="
                                );


                                // Finaliza correctamente
                                // el evento asíncrono.
                                return Task.CompletedTask;
                            },


                        // Este evento se ejecuta
                        // cuando el token sí es válido.
                        OnTokenValidated =
                            context =>
                            {
                                // Muestra en consola
                                // que el token fue aceptado.
                                Console.WriteLine(
                                    "JWT validado correctamente."
                                );

                                // Finaliza el evento.
                                return Task.CompletedTask;
                            }
                    };
            });


        // Activa el sistema de autorización
        // para poder utilizar [Authorize].
        builder.Services
            .AddAuthorization();


        // =========================================================
        // CONEXIÓN CON SQL SERVER
        // =========================================================

        // Registra el contexto de Entity Framework.
        builder.Services
            .AddDbContext<TiendaOnlineContext>(
                options =>
                    options.UseSqlServer(
                        builder.Configuration
                            .GetConnectionString(
                                "TiendaOnlineConnection"
                            )
                    )
            );


        // =========================================================
        // CONTROLADORES Y JSON
        // =========================================================

        // Registra los controladores de la API.
        builder.Services
            .AddControllers(options =>
            {
                // Evita que ASP.NET Core obligue
                // automáticamente a validar como required
                // todas las propiedades string no anulables.
                options
                    .SuppressImplicitRequiredAttributeForNonNullableReferenceTypes =
                    true;
            })

            // Configura el comportamiento de JSON.
            .AddJsonOptions(options =>
            {
                // Evita errores cuando existen
                // relaciones circulares entre entidades.
                options
                    .JsonSerializerOptions
                    .ReferenceHandler =
                    ReferenceHandler.IgnoreCycles;
            });


        // =========================================================
        // SWAGGER
        // =========================================================

        // Permite que Swagger detecte los endpoints.
        builder.Services
            .AddEndpointsApiExplorer();


        // Configura Swagger con soporte para JWT.
        builder.Services
            .AddSwaggerGen(options =>
            {
                // Define el sistema de seguridad Bearer.
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        // Nombre del encabezado HTTP.
                        Name = "Authorization",

                        // Indica que se utiliza
                        // autenticación HTTP.
                        Type =
                            SecuritySchemeType.Http,

                        // Define el esquema Bearer.
                        Scheme = "bearer",

                        // Indica que el contenido
                        // corresponde a un JWT.
                        BearerFormat = "JWT",

                        // El token se envía
                        // dentro del header.
                        In =
                            ParameterLocation.Header,

                        // Explicación que aparece
                        // dentro de Swagger.
                        Description =
                            "Pegue únicamente el token JWT"
                    }
                );


                // Hace que Swagger utilice
                // la definición Bearer anterior.
                options.AddSecurityRequirement(
                    document =>
                        new OpenApiSecurityRequirement
                        {
                            [
                                new OpenApiSecuritySchemeReference(
                                    "Bearer",
                                    document
                                )
                            ] =
                            new List<string>()
                        }
                );
            });


        // =========================================================
        // CORS PARA ANGULAR
        // =========================================================

        // Configura CORS para permitir
        // peticiones desde el proyecto Angular.
        builder.Services
            .AddCors(options =>
            {
                // Crea una política llamada Angular.
                options.AddPolicy(
                    "Angular",
                    policy =>
                    {
                        policy

                            // Permite peticiones
                            // desde Angular en localhost:4200.
                            .WithOrigins(
                                "http://localhost:4200"
                            )

                            // Permite cualquier encabezado.
                            .AllowAnyHeader()

                            // Permite GET, POST, PUT,
                            // DELETE y otros métodos.
                            .AllowAnyMethod();
                    }
                );
            });


        // =========================================================
        // CREAR LA APLICACIÓN
        // =========================================================

        // Construye la aplicación
        // con todas las configuraciones anteriores.
        var app =
            builder.Build();


        // Activa el manejador global
        // de excepciones.
        app.UseExceptionHandler();


        // =========================================================
        // SWAGGER EN DESARROLLO
        // =========================================================

        // Swagger solamente se activa
        // cuando la aplicación está en desarrollo.
        if (app.Environment.IsDevelopment())
        {
            // Genera el archivo Swagger.
            app.UseSwagger();

            // Muestra la interfaz Swagger UI.
            app.UseSwaggerUI();
        }


        // =========================================================
        // PIPELINE DE LA API
        // =========================================================

        // Redirige peticiones HTTP hacia HTTPS.
        app.UseHttpsRedirection();


        // Lee y valida el token JWT
        // enviado por el usuario.
        app.UseAuthentication();


        // Activa la política CORS
        // creada para Angular.
        app.UseCors("Angular");


        // Comprueba si el usuario
        // tiene permiso para acceder al endpoint.
        app.UseAuthorization();


        // Conecta las rutas
        // con los controladores.
        app.MapControllers();


        // Inicia la API
        // y queda esperando peticiones.
        app.Run();
    }
}