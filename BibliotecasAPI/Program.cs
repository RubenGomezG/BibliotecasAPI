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

//área de servicios

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

//área de middlewares

app.MapControllers();

app.Run();
