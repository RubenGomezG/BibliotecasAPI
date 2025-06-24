using AutoMapper;
using BibliotecasAPI.DAL.DTOs.AutorDTOs;
using BibliotecasAPI.DAL.DTOs.ComentarioDTOs;
using BibliotecasAPI.DAL.DTOs.LibroDTOs;
using BibliotecasAPI.DAL.DTOs.LlaveDTOs;
using BibliotecasAPI.DAL.DTOs.RestriccionDTOs.RestriccionDominioDTOs;
using BibliotecasAPI.DAL.DTOs.RestriccionDTO.RestriccionIpDTOs;
using BibliotecasAPI.DAL.DTOs.UsuarioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;

namespace BibliotecasAPI.Utils.MappingProfiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Autor, AutorDTO>()
                .ForMember(dto => dto.NombreCompleto, 
                    config => config.MapFrom(autor => MapearNombreCompletoAutor(autor)));
            CreateMap<Autor, AutorConLibrosDTO>()
                .ForMember(dto => dto.NombreCompleto,
                    config => config.MapFrom(autor => MapearNombreCompletoAutor(autor)));

            CreateMap<AutorCreacionDTO, Autor>();
            CreateMap<AutorCreacionConFotoDTO, Autor>()
                .ForMember(a => a.Foto, config => config.Ignore());
            CreateMap<Autor, AutorPatchDTO>().ReverseMap();            
            CreateMap<AutorLibro, LibroDTO>()
                .ForMember(dto => dto.Id, config => config.MapFrom(libro => libro.LibroId))
                .ForMember(dto => dto.Titulo, config => config.MapFrom(libro => libro.Libro!.Titulo));

            CreateMap<Libro, LibroDTO>();
            CreateMap<LibroCreacionDTO, Libro>()
                .ForMember(dto => dto.Autores, config => config
                .MapFrom(dto => dto.AutoresIds
                .Select(id => new AutorLibro { AutorId = id })));

            CreateMap<Libro, LibroConAutoresDTO>();
            CreateMap<AutorLibro, AutorDTO>()
                .ForMember(dto => dto.Id, config => config.MapFrom(autor => autor.AutorId))
                .ForMember(dto => dto.NombreCompleto, config => config.MapFrom(autor => MapearNombreCompletoAutor(autor.Autor!)));

            CreateMap<LibroCreacionDTO, AutorLibro>()
                .ForMember(ent => ent.Libro, config => 
                config.MapFrom(dto => new Libro { Titulo = dto.Titulo }));

            CreateMap<ComentarioCreacionDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>()
                .ForMember(dto => dto.UsuarioEmail, config => config.MapFrom(c => c.Usuario!.Email));
            CreateMap<Comentario, ComentarioConLibroDTO>();
            CreateMap<Comentario, ComentarioPatchDTO>().ReverseMap();
            CreateMap<Usuario, UsuarioDTO>();            
            CreateMap<LlaveAPI, LlaveDTO>();            
            CreateMap<RestriccionDominio, RestriccionDominioDTO>();            
            CreateMap<RestriccionIp, RestriccionIpDTO>();            
        }

        private string MapearNombreCompletoAutor(Autor autor) => $"{autor!.Nombre} {autor!.Apellidos}";
    }
}
