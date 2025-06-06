using BibliotecasAPI.BLL.Interfaces.IRepositories;
using BibliotecasAPI.BLL.Interfaces.IServices;
using BibliotecasAPI.BLL.Interfaces.IServices.V1;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.DTOs.ComentarioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.BLL.Impl.Services.V1
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

        public async Task<ActionResult> PatchComentario(Comentario comentarioDB, ComentarioPatchDTO comentarioPatchDTO)
        {
            return await _repositorioComentarios.PatchComentario(comentarioDB, comentarioPatchDTO);
        }

        public async Task<ActionResult> BorradoLogicoComentario(Guid id, int libroId)
        {
            return await _repositorioComentarios.BorradoLogicoComentario(id, libroId);
        }
    }
}
