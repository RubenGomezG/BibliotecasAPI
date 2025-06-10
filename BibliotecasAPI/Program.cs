using BibliotecasAPI.BLL.Repositories.Impl;
using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Impl;
using BibliotecasAPI.BLL.Services.Impl.V1;
using BibliotecasAPI.BLL.Services.Impl.V2;
using BibliotecasAPI.BLL.Services.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.BLL.Services.Interfaces.V2;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Filters;
using BibliotecasAPI.Utils.Filters.V1;
using BibliotecasAPI.Utils.Middlewares;
using BibliotecasAPI.Utils.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//área de servicios

//builder.Services.AddStackExchangeRedisOutputCache(opciones =>
//{
//    opciones.Configuration = builder.Configuration.GetConnectionString("Redis");
//});

builder.Services.AddOutputCache(opciones =>
{
    opciones.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(60);
});

builder.Services.AddDataProtection();
var allowedHosts = builder.Configuration.GetSection("origenesPermitidos").Get<string[]>()!;

builder.Services.AddCors(opciones => 
{
    opciones.AddDefaultPolicy(opcionesCORS =>
    {
        opcionesCORS.WithOrigins(allowedHosts)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("cantidad-registros");
    });
});
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers(opciones =>
{
    opciones.Conventions.Add(new ConvencionAgrupaPorVersion());
}).AddNewtonsoftJson();

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddIdentityCore<Usuario>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<Usuario>>();
builder.Services.AddScoped<SignInManager<Usuario>>();
builder.Services.AddScoped<FiltroValidacionLibro>();
builder.Services.AddScoped<IGeneradorEnlaces, GeneradorEnlaces>();
builder.Services.AddScoped<HateoasAutorAttribute>();
builder.Services.AddScoped<HateoasAutoresAttribute>();

builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();
builder.Services.AddTransient<IServicioAutores, ServicioAutores>();
builder.Services.AddTransient<IServicioAutoresV2, ServicioAutoresV2>();
builder.Services.AddTransient<IServicioComentarios, ServicioComentarios>();
builder.Services.AddTransient<IServicioAutoresColeccion, ServicioAutoresColeccion>();
builder.Services.AddTransient<IServicioLibros, ServicioLibros>();
builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();

builder.Services.AddTransient<IRepositorioUsuarios, RepositorioUsuarios>();
builder.Services.AddTransient<IRepositorioAutores, RepositorioAutores>();
builder.Services.AddTransient<IRepositorioAutoresColeccion, RepositorioAutoresColeccion>();
builder.Services.AddTransient<IRepositorioComentarios, RepositorioComentarios>();
builder.Services.AddTransient<IRepositorioLibros, RepositorioLibros>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication().AddJwtBearer(opciones =>
{
    opciones.MapInboundClaims = false;
    opciones.TokenValidationParameters = new TokenValidationParameters 
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["LlaveJWT"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(opciones => 
{
    opciones.AddPolicy("esAdmin", policy => policy.RequireClaim("esAdmin"));
});

builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Biblioteca API",
        Description = "Este es un web API para trabajar con datos de autores y libros",
        Contact = new OpenApiContact
        {
            Email = "Rubeng@gmail.com",
            Name = "Ruben Gomez",
            Url = new Uri("https://www.linkedin.com/in/rubengomezgarc/")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/license/mit/")
        }
    });

    opciones.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2",
        Title = "Biblioteca API",
        Description = "Este es un web API para trabajar con datos de autores y libros",
        Contact = new OpenApiContact
        {
            Email = "Rubeng@gmail.com",
            Name = "Ruben Gomez",
            Url = new Uri("https://www.linkedin.com/in/rubengomezgarc/")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/license/mit/")
        }
    });

    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    opciones.OperationFilter<FiltroAutorizacion>();
    //opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "Bearer"
    //            }
    //        },
    //        new string[]{}
    //    }
    //});
});
var app = builder.Build();

//área de middlewares

app.UseErrorHandler();
app.UseSwagger();
app.UseSwaggerUI(opciones =>
{
    opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "Biblioteca API V1");
    opciones.SwaggerEndpoint("/swagger/v2/swagger.json", "Biblioteca API V2");
});

app.UseStaticFiles();

app.UseCors();
app.UseOutputCache();
app.MapControllers();

app.Run();