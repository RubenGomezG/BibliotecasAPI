using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.Model.Entidades;

namespace BibliotecasAPI.BLL.Repositories.Impl
{
    public class RepositorioLlaves : IRepositorioLlaves
    {
        private readonly ApplicationDbContext _context;

        public RepositorioLlaves(ApplicationDbContext context)
        {
            _context = context;
        }
        public string GenerarLlave() => Guid.NewGuid().ToString().Replace("-", "");

        public async Task<LlaveAPI> CrearLlave(string usuarioId, TipoLlave tipoLlave)
        {
            var llave = GenerarLlave();

            var llaveAPI = new LlaveAPI
            {
                Llave = llave,
                TipoLlave = tipoLlave,
                Activa = true,
                UsuarioId = usuarioId
            };

            _context.Add(llaveAPI);
            await _context.SaveChangesAsync();
            return llaveAPI;
        }
    }
}
