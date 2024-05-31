using Microsoft.AspNetCore.Mvc;
using CSharpDevelopment.Models;
using CSharpDevelopment.Proxies;
using CSharpDevelopment.Services.SourcesService;

namespace CSharpDevelopment.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService = new UserServiceProxy();
    
    [HttpGet("{requestingLogin}/GetActiveUsers")]
    public async Task<ActionResult<List<User>>> GetActiveUsers(string requestingLogin, string requestingPassword)
    {
        if (!await _userService.AdminConfirmationAsync(requestingLogin, requestingPassword))
            return BadRequest("Login or password contains an error!");
        
        return await _userService.GetActiveUsersAsync();
    }

    private async Task<ActionResult<User>> CheckAndGetUserAsync(string login, 
        string errorMassage = "User has not been created!")
    {
        var user = await _userService.GetUserAsync(login);

        return user == null ? NotFound(errorMassage) : user;
    }
    
    [HttpGet("{requestingLogin}/{login}/GetUser")]
    public async Task<ActionResult<User>> GetUser(string requestingLogin, string requestingPassword, string login)
    {
        if (!await _userService.AdminConfirmationAsync(requestingLogin, requestingPassword))
            return BadRequest("Login or password contains an error!");

        return await CheckAndGetUserAsync(login);
    }
    
    [HttpGet("{requestingLogin}/GetUser")]
    public async Task<ActionResult<User>> GetUser(string requestingLogin, string requestingPassword)
    {
        if (!await _userService.UserConfirmationAsync(requestingLogin, requestingPassword))
            return BadRequest("Login or password contains an error!");

        return await CheckAndGetUserAsync(requestingLogin);
    }

    [HttpGet("{requestingLogin}/GetUsersOverCertainAge")]
    public async Task<ActionResult<List<User>>> GetUsersOverCertainAge(string requestingLogin, 
        string requestingPassword, uint certainAge)
    {
        if (!await _userService.AdminConfirmationAsync(requestingLogin, requestingPassword))
            return BadRequest("Login or password contains an error!");
        
        return await _userService.GetUsersOverCertainAgeAsync(certainAge);
    }
    
    [HttpPost("{requestingLogin}/CreateUser")]
    public async Task<ActionResult<User>> CreateUser(string requestingLogin, string requestingPassword, 
        string login, string password, string name, int gender, DateTime birthday, bool isAdmin)
    {
        if (!await _userService.AdminConfirmationAsync(requestingLogin, requestingPassword))
            return BadRequest("Login or password contains an error!");

        await _userService.CreateUserAsync(login, password, name, gender, birthday, isAdmin, requestingLogin);
        
        return await CheckAndGetUserAsync(login);
    }

    private async Task<ActionResult<User>> BaseOfUpdate(string requestingLogin, string requestingPassword, 
        string checkLogin, string getLogin, Func<Task> funcUpdate)
    {
        if (requestingLogin == checkLogin)
        {
            if (!await _userService.UserConfirmationAsync(requestingLogin, requestingPassword))
                return BadRequest("Login or password contains an error!");
        }
        else if (!await _userService.AdminConfirmationAsync(requestingLogin, requestingPassword))
            return BadRequest("Login or password contains an error!");

        await funcUpdate();
        
        return await CheckAndGetUserAsync(getLogin);
    }

    private async Task<ActionResult<User>> BaseOfUpdate(string requestingLogin, string requestingPassword,
        string login, Func<Task> funcUpdate) =>
        await BaseOfUpdate(requestingLogin, requestingPassword, login, login, funcUpdate);

    [HttpPatch("{requestingLogin}/UpdateDataUser")]
    public async Task<ActionResult<User>> UpdateDataUser(string requestingLogin, string requestingPassword,
        string login, string name, int gender, DateTime birthday) =>
        await BaseOfUpdate(requestingLogin, requestingPassword, login, async () =>
            await _userService.UpdateDataUserAsync(login, name, gender, birthday, requestingLogin));

    [HttpPatch("{requestingLogin}/UpdateLoginUser")]
    public async Task<ActionResult<User>> UpdateLoginUser(string requestingLogin, string requestingPassword, 
        string oldLogin, string newLogin) =>
        await BaseOfUpdate(requestingLogin, requestingPassword, oldLogin, newLogin, 
            async () => await _userService.UpdateLoginUserAsync(oldLogin, newLogin, requestingLogin));
    
    [HttpPatch("{requestingLogin}/UpdatePasswordUser")]
    public async Task<ActionResult<User>> UpdatePasswordUser(string requestingLogin, string requestingPassword, 
        string login, string newPassword) =>
        await BaseOfUpdate(requestingLogin, requestingPassword, login, async () => 
            await _userService.UpdatePasswordUserAsync(login, newPassword, requestingLogin));

    private async Task<IActionResult> BaseOfAction(string requestingLogin, string requestingPassword,
        Func<Task> func, string outputMessage)
    {
        if (!await _userService.AdminConfirmationAsync(requestingLogin, requestingPassword))
            return BadRequest("Login or password contains an error!");

        await func();
        
        return Ok(outputMessage);
    }
    
    [HttpDelete("{requestingLogin}/{login}/SoftDeleteUser")]
    public async Task<IActionResult> SoftDeleteUser(string requestingLogin, string requestingPassword, string login) =>
        await BaseOfAction(requestingLogin, requestingPassword,
            () => _userService.SoftDeleteUser(login, requestingLogin),
            "User has been successfully deleted!");

    [HttpDelete("{requestingLogin}/{login}/HardDeleteUser")]
    public async Task<IActionResult> HardDeleteUser(string requestingLogin, string requestingPassword, string login) =>
        await BaseOfAction(requestingLogin, requestingPassword,
            () => _userService.HardDeleteUser(login),
            "User has been successfully full deleted!");

    [HttpPut("{requestingLogin}/RecoveryUser")]
    public async Task<IActionResult> RecoveryUser(string requestingLogin, string requestingPassword, string login) =>
        await BaseOfAction(requestingLogin, requestingPassword,
            () => _userService.RecoveryUser(login),
            "User has been successfully recovered!");
}