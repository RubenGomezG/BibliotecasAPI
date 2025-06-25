using BibliotecasAPI.DAL.DTOs.UsuarioDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BibliotecasAPI.Utils.ClassUtils
{
    public static class UserUtils
    {
        public static async Task<RespuestaAutenticacionDTO> ConstruirToken(CredencialesUsuarioDTO credencialesUsuarioDTO,
            IConfiguration configuration, UserManager<Usuario> _userManager, string usuarioId)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("email", credencialesUsuarioDTO.Email),
                new Claim("usuarioId", usuarioId)
            };

            Usuario? usuario = await _userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);
            IList<Claim> claimsDB = await _userManager.GetClaimsAsync(usuario!);
            claims.AddRange(claimsDB);

            SymmetricSecurityKey llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["LlaveJWT"]!));
            SigningCredentials credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            DateTime expiracion = DateTime.UtcNow.AddYears(1);

            JwtSecurityToken tokenDeSeguridad = new JwtSecurityToken(issuer: null, audience: null,
                claims: claims, expires: expiracion, signingCredentials: credenciales);

            string token = new JwtSecurityTokenHandler().WriteToken(tokenDeSeguridad);
            return new RespuestaAutenticacionDTO
            {
                Token = token,
                FechaExpiracion = expiracion,
            };
        }

        public static bool ExisteUsuario(Usuario? usuario)
        {
            return usuario != null;
        }
    }
}
