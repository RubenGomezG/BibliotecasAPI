﻿using BibliotecasAPI.DAL.Datos;
using BibliotecasAPI.DAL.DTOs.LlaveDTOs;
using BibliotecasAPI.DAL.DTOs.PeticionDTOs;
using BibliotecasAPI.DAL.Model.Entidades;
using BibliotecasAPI.Utils.Attributes;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Net;

namespace BibliotecasAPI.Utils.Middlewares
{
    public class LimitarPeticionesMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IOptionsMonitor<LimitarPeticionesDTO> _optionsMonitor;

        public LimitarPeticionesMiddleware(RequestDelegate next, IOptionsMonitor<LimitarPeticionesDTO> optionsMonitor)
        {
            this.next = next;
            _optionsMonitor = optionsMonitor;
        }

        public async Task InvokeAsync(HttpContext httpContext, ApplicationDbContext dbContext)
        {
            if (EndpointIgnoraPeticiones(httpContext))
            {
                await next(httpContext);
                return;
            }

            LlaveAPI? llaveDB = await ObtenerLlaveBBDD(httpContext, dbContext);
            if (llaveDB == null)
            {
                return;
            }

            Peticion peticion = new Peticion { LlaveId = llaveDB.Id, FechaPeticion = DateTime.UtcNow };
            dbContext.Add(peticion);
            await dbContext.SaveChangesAsync();

            await next(httpContext);
        }

        private bool PeticionSuperaRestricciones(LlaveAPI llaveAPI, HttpContext httpContext)
        {
            bool tieneRestricciones = llaveAPI.RestriccionesDominio!.Count > 0 || llaveAPI.RestriccionesIp!.Count > 0;

            if (!tieneRestricciones)
            {
                return true;
            }

            bool peticionSuperaRestriccionesDominio = PeticionSuperaRestriccionesDominio(llaveAPI.RestriccionesDominio, httpContext);
            bool peticionSuperaRestriccionesIp = PeticionSuperaRestriccionesIp(llaveAPI.RestriccionesIp!, httpContext);

            return peticionSuperaRestriccionesDominio || peticionSuperaRestriccionesIp;
        }

        private bool PeticionSuperaRestriccionesDominio(List<RestriccionDominio> restricciones, HttpContext httpContext)
        {
            if (restricciones == null  || restricciones.Count == 0)
            {
                return false;
            }

            string referer = httpContext.Request.Headers.Referer.ToString();

            if (string.IsNullOrEmpty(referer))
            {
                return false;
            }

            Uri uri = new Uri(referer);
            string dominio = uri.Host;

            bool superaRestriccion = restricciones.Any(restriccion => restriccion.Dominio == dominio);
            return superaRestriccion;
        }

        private bool PeticionSuperaRestriccionesIp(List<RestriccionIp> restricciones, HttpContext httpContext)
        {
            if (restricciones == null || restricciones.Count == 0)
            {
                return false;
            }
            IPAddress? remoteIpAddress = httpContext.Connection.RemoteIpAddress;

            if (remoteIpAddress == null || string.IsNullOrEmpty(remoteIpAddress.ToString()))
            {
                return false;
            }

            bool superaRestriccion = restricciones.Any(restriccion => restriccion.Ip == remoteIpAddress.ToString());
            return superaRestriccion;
        }

        private async Task<LlaveAPI> ObtenerLlaveBBDD(HttpContext httpContext, ApplicationDbContext dbContext)
        {
            LimitarPeticionesDTO limitarPeticionesDTO = _optionsMonitor.CurrentValue;

            StringValues llaveStringValues = httpContext.Request.Headers["X-Api-Key"];

            if (llaveStringValues.Count == 0)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Debe proveer la ApiKey en la cabecera X-Api-Key");
                return null!;
            }

            if (llaveStringValues.Count > 1)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Solo debe haber una ApiKey en la cabecera X-Api-Key");
                return null!;
            }

            string? llave = llaveStringValues[0];

            LlaveAPI? llaveDB = await dbContext.LlavesAPI
                .Include(llave => llave.RestriccionesDominio)
                .Include(llave => llave.RestriccionesIp)
                .Include(llave => llave.Usuario)
                .FirstOrDefaultAsync(x => x.Llave == llave);

            if (llaveDB == null)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("La llave no existe");
                return null!;
            }

            if (!llaveDB.Activa)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("La llave se encuentra inactiva");
                return null!;
            }

            if (!PeticionSuperaRestricciones(llaveDB, httpContext))
            {
                httpContext.Response.StatusCode = 403;
                return null!;
            }

            if (llaveDB.TipoLlave == TipoLlave.Gratuita)
            {
                DateTime hoy = DateTime.UtcNow.Date;
                int peticionesHoy = await dbContext.Peticiones.CountAsync(peticiones => peticiones.LlaveId == llaveDB.Id && peticiones.FechaPeticion >= hoy);

                if (limitarPeticionesDTO.PeticionesPorDiaGratuito <= peticionesHoy)
                {
                    httpContext.Response.StatusCode = 429;
                    await httpContext.Response.WriteAsync("Ha excedido el límite de peticiones por hoy. Si desea poder realizar más, actualice a nivel profesional");
                    return null!;
                }
            }
            else if (llaveDB.Usuario!.TieneDeuda)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Debe pagar su suscripción profesional para obtener sus beneficios.");
                return null!;
            }

            return llaveDB;
        }

        private bool EndpointIgnoraPeticiones(HttpContext httpContext)
        {
            Endpoint? endpoint = httpContext.GetEndpoint();
            if (endpoint == null)
            {
                return true;
            }

            ControllerActionDescriptor? actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

            if (actionDescriptor != null)
            {
                bool accionIgnoraLimitePeticiones = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(DeshabilitarLimitePeticionesAttribute), true).Any();
                bool controladorIgnoraLimitePeticiones = actionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(DeshabilitarLimitePeticionesAttribute), true).Any();

                if (accionIgnoraLimitePeticiones || controladorIgnoraLimitePeticiones)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
