using BibliotecasAPI.Utils.OpcionesConfiguraciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace BibliotecasAPI.Controllers
{
    [ApiController]
    [Route("api/configuraciones")]
    public class ConfiguracionesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection seccion1;
        private readonly IConfigurationSection seccion2;
        private readonly PersonaOpciones _opcionesPersona;
        private readonly PagosProcesamiento _pagosProcesamiento;

        public ConfiguracionesController(IConfiguration configuration,
            IOptionsSnapshot<PersonaOpciones> opcionesPersona,
            PagosProcesamiento pagosProcesamiento)
        {
            this._configuration = configuration;
            seccion1 = configuration.GetSection("Seccion_1");
            seccion2 = configuration.GetSection("Seccion_1");
            this._opcionesPersona = opcionesPersona.Value;
            this._pagosProcesamiento = pagosProcesamiento;
        }

        [HttpGet("optionsMonitor")]
        public ActionResult GetTarifas()
        {
            return Ok(_pagosProcesamiento.ObtenerTarifas());
        }

        [HttpGet("seccion1opciones")]
        public ActionResult<string> GetSeccionOpcionesPersona()
        {
            return Ok(_opcionesPersona);
        }

        [HttpGet("seccionTarifaopciones")]
        public ActionResult<string> GetSeccionOpcionesTarifa()
        {
            return Ok(_opcionesPersona);
        }

        [HttpGet("proveedores")]
        public ActionResult<string> GetProveedor()
        {
            var valor = _configuration.GetValue<string>("Quien_Soy");

            return Ok(new { valor });
        }

        [HttpGet("obtenerTodos")]
        public ActionResult<string> GetObtenerTodos()
        {
            var hijos = _configuration.GetChildren().Select(c => $"{c.Key} {c.Value}");

            return Ok(new {hijos});
        }

        [HttpGet("seccion1")]
        public ActionResult<string> GetSeccion1()
        {
            string nombre = seccion1.GetValue<string>("Nombre")!;
            int edad = seccion1.GetValue<int>("Edad");

            return Ok( new { Nombre = nombre, Edad = edad });
        }

        [HttpGet("seccion2")]
        public ActionResult<string> GetSeccion2()
        {
            string nombre = seccion2.GetValue<string>("Nombre")!;
            int edad = seccion2.GetValue<int>("Edad");

            return Ok(new { Nombre = nombre, Edad = edad });
        }

        [HttpGet]
        public ActionResult<string> Get() 
        {
            string? opcion1 = _configuration["Apellido"];

            string? opcion2 = _configuration.GetValue<string>("Apellido")!;
            return opcion2;
        }

        [HttpGet("secciones")]
        public ActionResult<string> GetSeccion()
        {
            string? opcion1 = _configuration["ConnectionStrings:DefaultConnection"];

            string? opcion2 = _configuration.GetValue<string>("ConnectionStrings:DefaultConnection")!;

            var seccion = _configuration.GetSection("ConnectionStrings");
            string? opcion3 = seccion["DefaultConnection"];
            return opcion2;
        }
    }
}
