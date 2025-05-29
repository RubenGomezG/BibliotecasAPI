using AutoMapper;
using BibliotecasAPI.DTO.AutorDTOs;
using BibliotecasAPI.DTO.LibroDTOs;
using BibliotecasAPI.Model.Entidades;

namespace BibliotecasAPI.Utils.MappingProfiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Autor, AutorDTO>()
                .ForMember(dto => dto.NombreCompleto, 
                    config => config.MapFrom(autor => MapearNombreYApellidoAutor(autor)));

            CreateMap<Autor, AutorConLibrosDTO>()
                .ForMember(dto => dto.NombreCompleto,
                    config => config.MapFrom(autor => MapearNombreYApellidoAutor(autor)));

            CreateMap<AutorCreacionDTO, Autor>();

            CreateMap<Libro, LibroDTO>();

            CreateMap<Libro, LibroConAutorDTO>()
                .ForMember(dto => dto.AutorNombre,
                config => config.MapFrom(libro => MapearNombreYApellidoAutor(libro.Autor!)));

            CreateMap<LibroCreacionDTO, Libro>();
        }

        private string MapearNombreYApellidoAutor(Autor autor) => $"{autor!.Nombre} {autor!.Apellidos}";
    }
}
