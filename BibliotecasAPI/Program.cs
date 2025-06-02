using BibliotecasAPI.BLL.IServices;
using BibliotecasAPI.BLL.Services;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.Model.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//�rea de servicios

builder.Services.AddDataProtection();
var allowedHosts = builder.Configuration.GetSection("origenesPermitidos").Get<string[]>()!;

builder.Services.AddCors(opciones => 
{
    opciones.AddDefaultPolicy(opcionesCORS =>
    {
        opcionesCORS.WithOrigins(allowedHosts)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("mi-cabecera");
    });
});
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddIdentityCore<Usuario>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<Usuario>>();
builder.Services.AddScoped<SignInManager<Usuario>>();
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();
builder.Services.AddTransient<IServicioHash, ServicioHash>();
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
var app = builder.Build();

//�rea de middlewares
app.Use(async(contexto, next) =>
{
    contexto.Response.Headers.Append("mi-cabecera", "valor");
    await next();
});

app.UseCors();
app.MapControllers();

app.Run();
