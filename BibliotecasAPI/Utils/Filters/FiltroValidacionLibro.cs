﻿using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.LibroDTOs;
using BibliotecasAPI.Utils.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace BibliotecasAPI.Utils.Filters
{
    public class FiltroValidacionLibro : IAsyncActionFilter
    {
        private readonly ApplicationDbContext _dbContext;
        private const string ERROR_NO_LIBROS_SIN_AUTOR = "No se puede crear un libro sin autores";

        public FiltroValidacionLibro(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.TryGetValue("libroCreacionDTO", out var value) ||
                value is not LibroCreacionDTO libroCreacionDTO)
            {
                context.ModelState.AddModelError(string.Empty, ERROR_NO_LIBROS_SIN_AUTOR);
                context.Result = context.ModelState.ConstruirProblemDetail(ERROR_NO_LIBROS_SIN_AUTOR);
                return;
            }
            if (libroCreacionDTO.AutoresIds == null || libroCreacionDTO.AutoresIds.Count == 0)
            {
                context.ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds), ERROR_NO_LIBROS_SIN_AUTOR);
                context.Result = context.ModelState.ConstruirProblemDetail(ERROR_NO_LIBROS_SIN_AUTOR);
                return;
            }

            List<int> autoresIdsExisten = await _dbContext.Autores
                                    .Where(a => libroCreacionDTO.AutoresIds.Contains(a.Id))
                                    .Select(x => x.Id)
                                    .ToListAsync();

            if (autoresIdsExisten.Count != libroCreacionDTO.AutoresIds.Count)
            {
                IEnumerable<int> autoresNoExisten = libroCreacionDTO.AutoresIds.Except(autoresIdsExisten);
                context.ModelState.AddModelError(nameof(libroCreacionDTO.AutoresIds), $"Los siguientes autores {string.Join(',', autoresNoExisten)} no existen.");
                context.Result = context.ModelState.ConstruirProblemDetail($"Los siguientes autores {string.Join(',', autoresNoExisten)} no existen.");
                return;
            }

            await next();
        }
    }
}
