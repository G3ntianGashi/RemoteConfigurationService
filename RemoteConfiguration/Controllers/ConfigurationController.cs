using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Services;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly DataContextDapper _dapper;

        public ConfigurationController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetConfigurations")]
        [ServiceFilter(typeof(ApiKeyAttributeService))]
        public IActionResult  GetConfigurations()
        {
            if (HttpContext.Items["ApiKey"] is not Guid apiKey)
            {
                // Se non è un GUID, esci o restituisci un errore
                return BadRequest("Invalid API key format. It should be a valid GUID.");
            }

            var sql = "SELECT Id, ApiKey, KeyIdentifier, ConfigData" +
                      " FROM RemoteConfigurationSchema.Configuration" +
                      " WHERE ApiKey = @ApiKey";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@ApiKey", apiKey, DbType.Guid);

            IEnumerable<Configuration> configurations = _dapper.LoadDataWithParameters<Configuration>(sql, sqlParameters);

            return Ok(configurations);
        }

        [HttpGet("GetConfiguration")]
        [ServiceFilter(typeof(ApiKeyAttributeService))]
        public IActionResult GetConfiguration([FromQuery]string keyIdentifier)
        {
            if (HttpContext.Items["ApiKey"] is not Guid apiKey)
            {
                // Se non è un GUID, esci o restituisci un errore
                return BadRequest("Invalid API key format. It should be a valid GUID.");
            }

            var sql = "SELECT Id, ApiKey, KeyIdentifier, ConfigData" +
                      " FROM RemoteConfigurationSchema.Configuration" +
                      " WHERE ApiKey = @ApiKey" +
                      " AND KeyIdentifier = @KeyIdentifier";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@ApiKey", apiKey, DbType.Guid);
            sqlParameters.Add("@KeyIdentifier", keyIdentifier, DbType.String);

            Configuration configuration = _dapper.LoadDataSingleWithParameters<Configuration>(sql, sqlParameters);

            return Ok(configuration);
        }

        [HttpPost("UpsertConfiguration")]
        [ServiceFilter(typeof(ApiKeyAttributeService))]
        public IActionResult UpsertConfiguration([FromBody]ConfigurationDto configurationDto)
        {
            if (HttpContext.Items["ApiKey"] is not Guid apiKey)
            {
                // Se non è un GUID, esci o restituisci un errore
                return BadRequest("Invalid API key format. It should be a valid GUID.");
            }

            var sql = "EXEC RemoteConfigurationSchema.spConfiguration_Upsert"
                    + " @ApiKey=@ApiKeyParameter"
                    + ", @KeyIdentifier=@KeyIdentifierParameter"
                    + ", @ConfigData=@ConfigDataParameter";

            // Parametri per la query
            var parameters = new DynamicParameters();
            parameters.Add("@ApiKeyParameter", apiKey, DbType.Guid);
            parameters.Add("@KeyIdentifierParameter", configurationDto.KeyIdentifier, DbType.String);
            parameters.Add("@ConfigDataParameter", configurationDto.ConfigData.ToString(), DbType.String);

            if (_dapper.ExecuteSqlWithParameters(sql, parameters))
            {
                return Ok();
            }
            else
            {
                throw new Exception("Failed to insert configuration");
            }
        }

        [HttpDelete("DeleteConfiguration")]
        [ServiceFilter(typeof(ApiKeyAttributeService))]
        public IActionResult DeleteConfiguration([FromQuery]string keyIdentifier)
        {
            if (HttpContext.Items["ApiKey"] is not Guid apiKey)
            {
                // Se non è un GUID, esci o restituisci un errore
                return BadRequest("Invalid API key format. It should be a valid GUID.");
            }
            
            var sql = "DELETE FROM RemoteConfigurationSchema.Configuration"
                    + " WHERE ApiKey = @ApiKey"
                    + " AND KeyIdentifier = @KeyIdentifier";

            // Parametri per la query
            var parameters = new DynamicParameters();
            parameters.Add("@ApiKey", apiKey, DbType.Guid);
            parameters.Add("@KeyIdentifier", keyIdentifier, DbType.String);

            if (_dapper.ExecuteSqlWithParameters(sql, parameters))
            {
                return Ok();
            }
            else
            {
                throw new Exception("Failed to delete configuration");
            }
        }
    }
}
