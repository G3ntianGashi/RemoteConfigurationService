using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using DotnetAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly UserService _userService;

    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _userService = new UserService(config);
    }

    [HttpGet("GetUsers")]
    public IEnumerable<AppUser> GetUsers()
    {
        string sql = @"EXEC RemoteConfigurationSchema.spUsers_Get";

        IEnumerable<AppUser> users = _dapper.LoadData<AppUser>(sql);
        return users;
    }

    [HttpGet("GetUser")]
    public IEnumerable<AppUser> GetUser([FromQuery]int userId)
    {
        string sql = @"EXEC RemoteConfigurationSchema.spUsers_Get @UserId=@UserIdParameter";

        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@UserIdParameter", userId, DbType.Int32 );

        IEnumerable<AppUser> users = _dapper.LoadDataWithParameters<AppUser>(sql, sqlParameters);
        return users;
    }

    [HttpGet("GetUsersByActiveStatus")]
    public IEnumerable<AppUser> GetUsersByActiveStatus([FromQuery]bool isActive)
    {
        string sql = @"EXEC RemoteConfigurationSchema.spUsers_Get @Active=@ActiveParameter";
        
        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@ActiveParameter", isActive, DbType.Boolean );

        IEnumerable<AppUser> users = _dapper.LoadDataWithParameters<AppUser>(sql, sqlParameters);
        return users;
    }
    
    [HttpPut("EditUser")]
    public IActionResult EditUser(EditUserDto user)
    {
        if (_userService.EditUser(user))
        {
            return Ok();
        } 

        throw new Exception("Failed to update user");
    }

    [HttpPut("EditEmail")]
    public IActionResult EditEmail(EditUserEmailDto userEmailDto)
    {
        if (_userService.EditEmail(userEmailDto))
        {
            return Ok();
        } 

        throw new Exception("Failed to edit email");
    }

    [HttpDelete("DeleteUser")]
    public IActionResult DeleteUser([FromQuery]int userId)
    {
        string sql = @"RemoteConfigurationSchema.spUser_Delete
            @UserId=@UserIdParameter";

        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);

        if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok();
        } 

        throw new Exception("Failed to delete user");
    }
}
