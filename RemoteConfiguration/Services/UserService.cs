using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;

namespace DotnetAPI.Services
{
    public class UserService
    {
        private readonly DataContextDapper _dapper;
        
        public UserService(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        public bool UpsertUser(AppUser user)
        {
            string sql = @"EXEC RemoteConfigurationSchema.spUser_Upsert
                @UserId = @UserIdParameter,
                @Email = @EmailParameter,
                @ApiKey = @ApiKeyParameter,
                @Active = @ActiveParameter,
                @FirstName = @FirstNameParameter, 
                @LastName = @LastNameParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParameter", user.UserId, DbType.Int32);
            sqlParameters.Add("@EmailParameter", user.Email, DbType.String);
            sqlParameters.Add("@ApiKeyParameter", user.ApiKey, DbType.Guid);
            sqlParameters.Add("@ActiveParameter", user.Active, DbType.Boolean);
            sqlParameters.Add("@FirstNameParameter", user.FirstName, DbType.String);
            sqlParameters.Add("@LastNameParameter", user.LastName, DbType.String);

            return _dapper.ExecuteSqlWithParameters(sql, sqlParameters);;
        }

        public bool EditUser(EditUserDto user)
        {
            string sql = @"EXEC RemoteConfigurationSchema.spUser_EditUser
                @UserId = @UserIdParameter,
                @Active = @ActiveParameter,
                @FirstName = @FirstNameParameter, 
                @LastName = @LastNameParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParameter", user.UserId, DbType.Int32);
            sqlParameters.Add("@ActiveParameter", user.Active, DbType.Boolean);
            sqlParameters.Add("@FirstNameParameter", user.FirstName, DbType.String);
            sqlParameters.Add("@LastNameParameter", user.LastName, DbType.String);

            return _dapper.ExecuteSqlWithParameters(sql, sqlParameters);;
        }

        public bool EditEmail(EditUserEmailDto userEmailDto)
        {
            string sql = @"EXEC RemoteConfigurationSchema.spUser_UpdateEmail
                @OldEmail = @OldEmailParameter,
                @NewEmail = @NewEmailParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@OldEmailParameter", userEmailDto.OldEmail, DbType.String);
            sqlParameters.Add("@NewEmailParameter", userEmailDto.NewEmail, DbType.String);

            return _dapper.ExecuteSqlWithParameters(sql, sqlParameters);;
        }
    }
}
