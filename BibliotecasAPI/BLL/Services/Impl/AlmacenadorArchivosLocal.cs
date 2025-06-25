using BibliotecasAPI.BLL.Services.Interfaces;

namespace BibliotecasAPI.BLL.Services.Impl
{
    public class AlmacenadorArchivosLocal : IAlmacenadorArchivos
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AlmacenadorArchivosLocal(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Almacenar(string? contenedor, IFormFile archivo)
        {
            string extension = Path.GetExtension(archivo.FileName);
            string nombreArchivo = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(_environment.WebRootPath, contenedor!);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string ruta = Path.Combine(folder, nombreArchivo);

            using (MemoryStream ms = new MemoryStream())
            {
                await archivo.CopyToAsync(ms);
                byte[] contenido = ms.ToArray();
                await File.WriteAllBytesAsync(ruta, contenido);
            }
            HttpRequest request = _httpContextAccessor.HttpContext!.Request;
            string url = $"{request.Scheme}://{request.Host}";
            string urlArchivo = Path.Combine(url, contenedor!, nombreArchivo).Replace("\\", "/");
            
            return urlArchivo;
        }

        public Task Borrar(string? ruta, string contenedor)
        {
            if (string.IsNullOrEmpty(ruta))
            {
                return Task.CompletedTask;
            }

            string nombreArchivo = Path.GetFileName(ruta);

            string directorioArchivo = Path.Combine(_environment.WebRootPath, contenedor, nombreArchivo);

            if (File.Exists(directorioArchivo)) 
            {
                File.Delete(directorioArchivo);
            }

            return Task.CompletedTask;
        }
    }
}
