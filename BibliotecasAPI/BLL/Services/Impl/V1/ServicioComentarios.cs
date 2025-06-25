using BibliotecasAPI.BLL.Repositories.Interfaces;
using BibliotecasAPI.BLL.Services.Interfaces.V1;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.DTOs.ComentarioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Services.Impl.V1
{
    public class ServicioComentarios : IServicioComentarios
    {
        private readonly IRepositorioComentarios _repositorioComentarios;

        public ServicioComentarios(IRepositorioComentarios repositorioComentarios)
        {
            _repositorioComentarios = repositorioComentarios;
        }

        public async Task<ActionResult<IEnumerable<ComentarioDTO>>> GetComentariosDeLibro(int libroId)
        {
            return await _repositorioComentarios.GetComentariosDeLibro(libroId);
        }

        public async Task<ActionResult<ComentarioConLibroDTO>> GetComentarioPorId(Guid id)
        {
            return await _repositorioComentarios.GetComentarioPorId(id);
        }

        public async Task<ActionResult> AnadirComentario(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            return await _repositorioComentarios.AnadirComentario(libroId, comentarioCreacionDTO);
        }
        public async Task<ActionResult> ActualizarComentario(int id, AutorCreacionDTO autorCreacionDTO)
        {
            return await _repositorioComentarios.ActualizarComentario(id, autorCreacionDTO);
        }

        public async Task<ActionResult> PatchComentario(Guid id, int libroId, JsonPatchDocument<ComentarioPatchDTO> patchDoc)
        {
            return await _repositorioComentarios.PatchComentario(id, libroId, patchDoc);
        }

        public async Task<ActionResult> BorradoLogicoComentario(Guid id, int libroId)
        {
            return await _repositorioComentarios.BorradoLogicoComentario(id, libroId);
        }
    }
}
