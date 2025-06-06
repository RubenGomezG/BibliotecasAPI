using BibliotecasAPI.BLL.Impl.Services;
using BibliotecasAPI.BLL.Impl.Services.V1;
using BibliotecasAPI.BLL.Impl.Services.V2;
using BibliotecasAPI.BLL.Interfaces.IServices;
using BibliotecasAPI.BLL.Interfaces.IServices.V1;
using BibliotecasAPI.BLL.Interfaces.IServices.V2;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Filters;
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
    opciones.Filters.Add<FiltroTiempoEjecucion>();
}).AddNewtonsoftJson();

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddIdentityCore<Usuario>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<Usuario>>();
builder.Services.AddScoped<SignInManager<Usuario>>();
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();
builder.Services.AddTransient<IServicioAutores, ServicioAutores>();
builder.Services.AddTransient<IServicioAutoresV2, ServicioAutoresV2>();
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();
builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddScoped<MiFiltroDeAccion>();
builder.Services.AddScoped<FiltroValidacionLibro>();

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
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseCors();
app.UseOutputCache();
app.MapControllers();

app.Run();