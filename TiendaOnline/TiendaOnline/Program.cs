// Configura autenticación JWT.
using Microsoft.AspNetCore.Authentication.JwtBearer;

// Permite usar Entity Framework Core.
using Microsoft.EntityFrameworkCore;

// Permite validar tokens JWT.
using Microsoft.IdentityModel.Tokens;

// Permite configurar Swagger.
using Microsoft.OpenApi;

// Permite configurar QuestPDF.
using QuestPDF.Infrastructure;

// Permite convertir la clave JWT a bytes.
using System.Text;

// Evita ciclos al serializar JSON.
using System.Text.Json.Serialization;

// Importa el contexto de datos.
using TiendaOnline.AccesoDatos.Context;

// Importa el manejador global de errores.
using TiendaOnline.API.Manejadores;

// Importa las configuraciones.
using TiendaOnline.Dominio.Configuracion;

// Importa las interfaces de negocio.
using TiendaOnline.Dominio.InterfacesLN;

// Importa los servicios.
using TiendaOnline.LogicaNegocio.Servicios;

public partial class Program
{
    private static void Main(string[] args)
    {
        // Crea la aplicación.
        var builder =
            WebApplication.CreateBuilder(args);

        // Registra el manejador de errores.
        builder.Services
            .AddExceptionHandler<ManejadorGlobalExcepciones>();

        // Agrega respuestas estándar de error.
        builder.Services
            .AddProblemDetails();

        // Lee la configuración JWT.
        builder.Services
            .Configure<JwtConfiguracion>(
                builder.Configuration
                    .GetSection("Jwt")
            );

        // Lee la configuración del correo.
        builder.Services
            .Configure<CorreoConfiguracion>(
                builder.Configuration
                    .GetSection("Correo")
            );

        // Registra el servicio JWT.
        builder.Services.AddScoped<
            IJwtServicio,
            JwtServicio
        >();

        // Registra el servicio de autenticación.
        builder.Services.AddScoped<
            IAuthServicio,
            AuthServicio
        >();

        // Registra el servicio de pedidos.
        builder.Services.AddScoped<
            IPedidoServicio,
            PedidoServicio
        >();

        // Registra el servicio que genera PDF.
        builder.Services.AddScoped<
            IPdfServicio,
            PdfServicio
        >();

        // Registra el servicio que envía correos.
        builder.Services.AddScoped<
            ICorreoServicio,
            CorreoServicio
        >();

        // Configura la licencia de QuestPDF.
        QuestPDF.Settings.License =
            LicenseType.Community;

        // Obtiene la configuración JWT.
        var jwtConfiguracion =
            builder.Configuration
                .GetSection("Jwt")
                .Get<JwtConfiguracion>();

        // Valida que exista la clave JWT.
        if (
            jwtConfiguracion == null ||
            string.IsNullOrWhiteSpace(
                jwtConfiguracion.Clave
            )
        )
        {
            throw new InvalidOperationException(
                "La configuración JWT no está completa."
            );
        }

        // Convierte la clave a bytes.
        var clave =
            Encoding.UTF8.GetBytes(
                jwtConfiguracion.Clave
            );

        // Configura autenticación JWT.
        builder.Services
            .AddAuthentication(options =>
            {
                // Esquema principal.
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults
                        .AuthenticationScheme;

                // Esquema para autorización.
                options.DefaultChallengeScheme =
                    JwtBearerDefaults
                        .AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Conserva el token.
                options.SaveToken = true;

                // Permite trabajar en desarrollo.
                options.RequireHttpsMetadata = false;

                // Reglas de validación del token.
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        // Valida emisor.
                        ValidateIssuer = true,

                        // Valida audiencia.
                        ValidateAudience = true,

                        // Valida expiración.
                        ValidateLifetime = true,

                        // Valida firma.
                        ValidateIssuerSigningKey = true,

                        // Emisor esperado.
                        ValidIssuer =
                            jwtConfiguracion.Emisor,

                        // Audiencia esperada.
                        ValidAudience =
                            jwtConfiguracion.Audiencia,

                        // Clave para validar la firma.
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                clave
                            ),

                        // Sin tiempo extra.
                        ClockSkew =
                            TimeSpan.Zero
                    };

                // Eventos para revisar el JWT.
                options.Events =
                    new JwtBearerEvents
                    {
                        // Muestra errores del token.
                        OnAuthenticationFailed =
                            context =>
                            {
                                Console.WriteLine(
                                    $"Error JWT: {context.Exception.Message}"
                                );

                                return Task.CompletedTask;
                            },

                        // Confirma token válido.
                        OnTokenValidated =
                            context =>
                            {
                                Console.WriteLine(
                                    "JWT validado correctamente."
                                );

                                return Task.CompletedTask;
                            }
                    };
            });

        // Activa autorización.
        builder.Services
            .AddAuthorization();

        // Configura SQL Server.
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

        // Registra controladores.
        builder.Services
            .AddControllers(options =>
            {
                // Evita required automático.
                options
                    .SuppressImplicitRequiredAttributeForNonNullableReferenceTypes =
                    true;
            })
            .AddJsonOptions(options =>
            {
                // Evita relaciones circulares.
                options
                    .JsonSerializerOptions
                    .ReferenceHandler =
                    ReferenceHandler.IgnoreCycles;
            });

        // Habilita Swagger.
        builder.Services
            .AddEndpointsApiExplorer();

        // Configura Swagger con JWT.
        builder.Services
            .AddSwaggerGen(options =>
            {
                // Define autenticación Bearer.
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        // Nombre del header.
                        Name = "Authorization",

                        // Tipo HTTP.
                        Type =
                            SecuritySchemeType.Http,

                        // Esquema Bearer.
                        Scheme = "bearer",

                        // Formato JWT.
                        BearerFormat = "JWT",

                        // Va en el header.
                        In =
                            ParameterLocation.Header,

                        // Ayuda para Swagger.
                        Description =
                            "Pegue únicamente el token JWT"
                    }
                );

                // Aplica seguridad Bearer.
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

        // Permite peticiones desde Angular.
        builder.Services
            .AddCors(options =>
            {
                // Política para localhost:4200.
                options.AddPolicy(
                    "Angular",
                    policy =>
                    {
                        policy
                            .WithOrigins(
                                "http://localhost:4200"
                            )
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                );
            });

        // Construye la aplicación.
        var app =
            builder.Build();

        // Activa manejo de errores.
        app.UseExceptionHandler();

        // Activa Swagger en desarrollo.
        if (
            app.Environment
                .IsDevelopment()
        )
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Redirige a HTTPS.
        app.UseHttpsRedirection();

        // Valida el token.
        app.UseAuthentication();

        // Activa CORS.
        app.UseCors("Angular");

        // Valida permisos.
        app.UseAuthorization();

        // Mapea controladores.
        app.MapControllers();

        // Inicia la API.
        app.Run();
    }
}