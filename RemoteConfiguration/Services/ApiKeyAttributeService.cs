using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Dapper;
using DotnetAPI.Data;
using System.Data;

namespace DotnetAPI.Services
{
    public class ApiKeyAttributeService : IAsyncActionFilter
    {
        private readonly DataContextDapper _dapper;

        // Inietta IConfiguration per accedere alla stringa di connessione
        public ApiKeyAttributeService(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Recupera l'header con l'API Key
            if (!context.HttpContext.Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401, // Unauthorized
                    Content = "API Key is missing"
                };
                return;
            }

            // Controlla se la chiave API è null o vuota
            if (string.IsNullOrWhiteSpace(extractedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401, // Unauthorized
                    Content = "API Key is invalid or missing"
                };
                return;
            }

            if (!Guid.TryParse(extractedApiKey, out Guid parsedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401, // Unauthorized
                    Content = "API Key is invalid Guid"
                };
                return;
            }

            // Verifica se l'API Key è valida
            if (!await IsApiKeyValid(parsedApiKey))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 403, // Forbidden
                    Content = "Unauthorized client"
                };
                return;
            }

            // Salva l'API Key validata nel contesto della richiesta
            context.HttpContext.Items["ApiKey"] = parsedApiKey;

            // Procedi con l'esecuzione dell'azione se l'API Key è valida
            await next();
        }

        private async Task<bool> IsApiKeyValid(Guid apiKey)
        {
            var query = "SELECT COUNT(1) FROM RemoteConfigurationSchema.Users WHERE ApiKey = @ApiKey"; // Cambia con la tua tabella
            
            var parameters = new DynamicParameters();
            parameters.Add("@ApiKey", apiKey, DbType.Guid);

            // Esegue la query per verificare l'esistenza dell'API Key
            int result = await _dapper.LoadDataWithRowCount(query, parameters);
            return result > 0;
        }
    }
}
