// Configura autenticación JWT.
using Microsoft.AspNetCore.Authentication.JwtBearer;

// Permite usar Entity Framework Core.
using Microsoft.EntityFrameworkCore;

// Permite validar tokens JWT.
using Microsoft.IdentityModel.Tokens;

// Permite configurar Swagger.
using Microsoft.OpenApi;

// Importa el perfil de AutoMapper.
using TiendaOnline.Dominio.DTO;

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

// Importa las implementaciones de lógica de negocio.
using TiendaOnline.LogicaNegocio.Implementaciones;

// Importa los servicios.
using TiendaOnline.LogicaNegocio.Servicios;

// Importa las interfaces de acceso a datos.
using TiendaOnline.Dominio.InterfacesAD;

// Importa las implementaciones de acceso a datos.
using TiendaOnline.AccesoDatos.Implementaciones;

public partial class Program
{
    private static void Main(string[] args)
    {
        // Crea la aplicación.
        var builder =
            WebApplication.CreateBuilder(args);

        // Registra AutoMapper y busca los perfiles del proyecto.
        builder.Services.AddAutoMapper(
            config => { },
            typeof(AutoMapperProfile)
        );

        // =====================================================
        // MANEJO DE ERRORES
        // =====================================================

        // Registra el manejador global de errores.
        builder.Services
            .AddExceptionHandler<ManejadorGlobalExcepciones>();

        // Agrega respuestas estándar de error.
        builder.Services
            .AddProblemDetails();


        // =====================================================
        // CONFIGURACIONES
        // =====================================================

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


        // =====================================================
        // SERVICIOS 
        // =====================================================

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

        // Registra el servicio especial de pedidos.
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


        // =====================================================
        // IMPLEMENTACIONES DE LÓGICA DE NEGOCIO
        // =====================================================

        // Registra la lógica de categorías.
        builder.Services.AddScoped<
            ICategoriaLN,
            CategoriaLN
        >();

        // Registra la lógica de compras a proveedores.
        builder.Services.AddScoped<
            ICompraProveedorLN,
            CompraProveedorLN
        >();

        // Registra la lógica de descuentos.
        builder.Services.AddScoped<
            IDescuentoLN,
            DescuentoLN
        >();

        // Registra la lógica de evaluaciones de productos.
        builder.Services.AddScoped<
            IEvaluacionProductoLN,
            EvaluacionProductoLN
        >();

        // Registra la lógica de inventario.
        builder.Services.AddScoped<
            IInventarioLN,
            InventarioLN
        >();

        // Registra la lógica de listas de deseos.
        builder.Services.AddScoped<
            IListaDeseoLN,
            ListaDeseoLN
        >();

        // Registra la lógica de movimientos de inventario.
        builder.Services.AddScoped<
            IMovimientoInventarioLN,
            MovimientoInventarioLN
        >();

        // Registra la lógica de notificaciones.
        builder.Services.AddScoped<
            INotificacionLN,
            NotificacionLN
        >();

        // Registra la lógica general de pedidos.
        builder.Services.AddScoped<
            IPedidoLN,
            PedidoLN
        >();

        // Registra la lógica de productos.
        builder.Services.AddScoped<
            IProductoLN,
            ProductoLN
        >();

        // Registra la lógica de proformas.
        builder.Services.AddScoped<
            IProformaLN,
            ProformaLN
        >();

        // Registra la lógica de usuarios.
        builder.Services.AddScoped<
            IUsuarioLN,
            UsuarioLN
        >();


        // =====================================================
        // QUESTPDF
        // =====================================================

        // Configura la licencia de QuestPDF.
        QuestPDF.Settings.License =
            LicenseType.Community;


        // =====================================================
        // JWT
        // =====================================================

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


        // =====================================================
        // AUTENTICACIÓN
        // =====================================================

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

                        // No agrega tiempo extra.
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

                        // Confirma que el token es válido.
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


        // =====================================================
        // BASE DE DATOS
        // =====================================================

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

        // =====================================================
        // UNIDAD DE TRABAJO
        // =====================================================

        // Registra la unidad de trabajo.
        builder.Services.AddScoped<
            IUnidadTrabajoEF,

            UnidadTrabajoEF
        >();

        // =====================================================
        // CONTROLADORES
        // =====================================================

        // Registra los controladores.
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


        // =====================================================
        // SWAGGER
        // =====================================================

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

                        // Se envía en el header.
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


        // =====================================================
        // CORS
        // =====================================================

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


        // =====================================================
        // CONSTRUCCIÓN DE LA APLICACIÓN
        // =====================================================

        // Construye la aplicación.
        var app =
            builder.Build();


        // =====================================================
        // MIDDLEWARE
        // =====================================================

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

        // Mapea los controladores.
        app.MapControllers();

        // Inicia la API.
        app.Run();
    }
}