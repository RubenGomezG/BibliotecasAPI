using BibliotecasAPI.DAL.DTOs.RestriccionDTO.RestriccionDominioDTOs;
using BibliotecasAPI.DAL.DTOs.RestriccionDTO.RestriccionIpDTOs;

namespace BibliotecasAPI.DAL.DTOs.LlaveDTOs
{
    public class LlaveDTO
    {
        public int Id { get; set; }
        public required string Llave { get; set; }
        public bool Activa { get; set; }
        public required string TipoLlave { get; set; }
        public List<RestriccionDominioDTO> RestriccionesDominio { get; set; } = [];
        public List<RestriccionIpDTO> RestriccionesIp { get; set; } = [];

    }
}
