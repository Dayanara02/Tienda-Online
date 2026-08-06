using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using TiendaOnline.LogicaNegocio.Interfaces;
using TiendaOnline.LogicaNegocio.Servicios;
using System.Text.Json.Serialization;
using TiendaOnline.AccesoDatos.Context;
using TiendaOnline.Dominio.Configuracion;
using TiendaOnline.LogicaNegocio.Interfaces;
using TiendaOnline.LogicaNegocio.Servicios;
using TiendaOnline.API.Manejadores;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<ManejadorGlobalExcepciones>();
builder.Services.AddProblemDetails();

builder.Services.Configure<JwtConfiguracion>(
    builder.Configuration.GetSection("Jwt")
);

builder.Services.AddScoped<IJwtServicio, JwtServicio>();
builder.Services.AddScoped<IAuthServicio, AuthServicio>();
builder.Services.AddScoped<IPedidoServicio, PedidoServicio>();

var jwtConfiguracion = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtConfiguracion>();

if (jwtConfiguracion == null ||
    string.IsNullOrWhiteSpace(jwtConfiguracion.Clave))
{
    throw new InvalidOperationException(
        "La configuración JWT no está completa."
    );
}

var clave = Encoding.UTF8.GetBytes(
    jwtConfiguracion.Clave
);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtConfiguracion.Emisor,
                ValidAudience = jwtConfiguracion.Audiencia,

                IssuerSigningKey =
                    new SymmetricSecurityKey(clave),

                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();

// CONEXIÓN CON SQL SERVER

builder.Services.AddDbContext<TiendaOnlineContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "TiendaOnlineConnection"
        )
    )
);

// CONTROLADORES Y CICLOS JSON


builder.Services.AddControllers(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes =
        true;
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
        ReferenceHandler.IgnoreCycles;
});

// SWAGGER CON JWT


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Pegue únicamente el token JWT"
        }
    );

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(
                "Bearer",
                document
            )] = new List<string>()
        }
    );
});

// CREAR LA APLICACIÓN
builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();
app.UseExceptionHandler();

// SWAGGER
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// PIPELINE
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseCors("Angular");
app.UseAuthorization();

app.MapControllers();

app.Run();