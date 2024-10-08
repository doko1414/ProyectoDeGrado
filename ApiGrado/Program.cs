using ApiGrado.Data;
using ApiGrado.Mappers;
using ApiGrado.Repositorio.IRepositorio;
using ApiGrado.Repositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Configuramos la conexión a SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
{
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql"));
});

// Añadir servicios al contenedor.

builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IAccesorioRepositorio, AccesorioRepositorio>(provider =>
    new AccesorioRepositorio(
        provider.GetRequiredService<ApplicationDbContext>(),
        provider.GetRequiredService<IWebHostEnvironment>()
    )
); builder.Services.AddScoped<IPedidosRepositorio, PedidoRepositorio>();

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 15 * 1024 * 1024; // 15 MB
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 15 * 1024 * 1024; // 15 MB
});

var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");

// Agregar AutoMapper
builder.Services.AddAutoMapper(typeof(BlogMapper));

// Configuración de la Autenticación
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrador"));
    options.AddPolicy("ClientOnly", policy => policy.RequireRole("cliente"));
});

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PolicyCors", build =>
    {
        build.AllowAnyOrigin()  // Permitir solicitudes de cualquier origen
             .AllowAnyMethod()  // Permitir cualquier método HTTP (GET, POST, etc.)
             .AllowAnyHeader(); // Permitir cualquier encabezado HTTP
    });
});

builder.Services.AddControllers().AddNewtonsoftJson();

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "Autenticación JWT usando el esquema Bearer. \r\n\r\n " +
        "Ingresa la palabra 'Bearer' seguida de un [espacio] y despues su token en el campo de abajo \r\n\r\n" +
        "Ejemplo: \"Bearer tkdknkdllskd\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configuración para servir archivos estáticos, incluyendo modelos 3D (.glb)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "ImagenesPosts")),
    RequestPath = "/ImagenesPosts",
    ContentTypeProvider = new FileExtensionContentTypeProvider(
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { ".glb", "model/gltf-binary" }
        }),
    OnPrepareResponse = ctx =>
    {
        // Habilitar CORS para archivos estáticos
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
    }
});

// Usar CORS
app.UseCors("PolicyCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
