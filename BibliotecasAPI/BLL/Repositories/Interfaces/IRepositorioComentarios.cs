using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.DTOs.ComentarioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecasAPI.BLL.Repositories.Interfaces
{
    public interface IRepositorioComentarios
    {
        public Task<ActionResult<IEnumerable<ComentarioDTO>>> GetComentariosDeLibro(int libroId);
        public Task<ActionResult<ComentarioConLibroDTO>> GetComentarioPorId(Guid id);
        public Task<ActionResult> AnadirComentario(int libroId, ComentarioCreacionDTO comentarioCreacionDTO);
        public Task<ActionResult> ActualizarComentario(int id, AutorCreacionDTO autorCreacionDTO);
        public Task<ActionResult> PatchComentario(Guid id, int libroId, JsonPatchDocument<ComentarioPatchDTO> patchDoc);
        public Task<ActionResult> BorradoLogicoComentario(Guid id, int libroId);
    }
}
