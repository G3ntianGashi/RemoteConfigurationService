using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using DotnetAPI.Dtos;
using System.Runtime.InteropServices;

namespace DotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IMapper _mapper;

        public ConfigurationController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);

            _mapper = new Mapper(new MapperConfiguration(cfg => 
            {
                //cfg.CreateMap<UserForRegistrationDto, AppUser>();
            }));
        }

        [HttpGet("GetConfigurations")]
        public IActionResult  GetConfigurations([FromQuery]string apiKey)
        {
            if (!Guid.TryParse(apiKey, out Guid parsedApiKey))
            {
                // Se la stringa non è un GUID valido, ritorna un errore 400 (Bad Request)
                return BadRequest("Invalid API key format. It should be a valid GUID.");
            }

            var sql = "SELECT Id, ApiKey, KeyIdentifier, ConfigData" +
                      " FROM RemoteConfigurationSchema.Configuration" +
                      " WHERE ApiKey = @ApiKey";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@ApiKey", parsedApiKey, DbType.Guid);

            IEnumerable<Configuration> configurations = _dapper.LoadDataWithParameters<Configuration>(sql, sqlParameters);

            return Ok(configurations);
        }

        [HttpGet("GetConfiguration")]
        public IActionResult GetConfiguration([FromQuery]string apiKey, [FromQuery]string keyIdentifier)
        {
            if (!Guid.TryParse(apiKey, out Guid parsedApiKey))
            {
                // Se la stringa non è un GUID valido, ritorna un errore 400 (Bad Request)
                return BadRequest("Invalid API key format. It should be a valid GUID.");
            }

            var sql = "SELECT Id, ApiKey, KeyIdentifier, ConfigData" +
                      " FROM RemoteConfigurationSchema.Configuration" +
                      " WHERE ApiKey = @ApiKey" +
                      " AND KeyIdentifier = @KeyIdentifier";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@ApiKey", parsedApiKey, DbType.Guid);
            sqlParameters.Add("@KeyIdentifier", keyIdentifier, DbType.String);

            Configuration configuration = _dapper.LoadDataSingleWithParameters<Configuration>(sql, sqlParameters);

            return Ok(configuration);
        }

        [HttpPost("InsertConfiguration")]
        public IActionResult InsertConfiguration([FromQuery]string apiKey, [FromBody]ConfigurationDto configurationDto)
        {
            if (!Guid.TryParse(apiKey, out Guid parsedApiKey))
            {
                // Se la stringa non è un GUID valido, ritorna un errore 400 (Bad Request)
                return BadRequest("Invalid API key format. It should be a valid GUID.");
            }

            var sql = "INSERT INTO RemoteConfigurationSchema.Configuration (ApiKey, KeyIdentifier, ConfigData)" 
                    + " VALUES (@ApiKey, @KeyIdentifier, @ConfigData);";

            // Parametri per la query
            var parameters = new DynamicParameters();
            parameters.Add("@ApiKey", parsedApiKey, DbType.Guid);
            parameters.Add("@KeyIdentifier", configurationDto.KeyIdentifier, DbType.String);
            parameters.Add("@ConfigData", configurationDto.ConfigData.ToString(), DbType.String);

            if (_dapper.ExecuteSqlWithParameters(sql, parameters))
            {
                return Ok();
            }
            else
            {
                throw new Exception("Failed to insert configuration");
            }
        }
    }
}
