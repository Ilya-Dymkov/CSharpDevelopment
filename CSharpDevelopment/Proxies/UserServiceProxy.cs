using CSharpDevelopment.Models;
using CSharpDevelopment.Services;
using CSharpDevelopment.Services.SourcesService;
using Microsoft.EntityFrameworkCore;

namespace CSharpDevelopment.Proxies;

public class UserServiceProxy : IUserService
{
    private readonly ILogger _logger = new Logger<UserServiceProxy>(LoggerFactory
        .Create(builder => builder.AddConsole()));
    
    private readonly IUserService _userService;

    private void LoggingData(string dataToLogging) => _logger.LogInformation(dataToLogging);

    public UserServiceProxy() => _userService = new UserService();

    public UserServiceProxy(Func<DbContextOptionsBuilder, DbContextOptionsBuilder> func) => 
        _userService = new UserService(func);

    public Task<List<User>> GetActiveUsersAsync()
    {
        LoggingData("Getting all active users.");
        
        return _userService.GetActiveUsersAsync();
    }

    public Task<User?> GetUserAsync(string login)
    {
        LoggingData($"Getting certain user({login}) by login.");
        
        return _userService.GetUserAsync(login);
    }

    public Task<List<User>> GetUsersOverCertainAgeAsync(uint certainAge)
    {
        LoggingData($"Getting all users over certain age({certainAge}).");
        
        return _userService.GetUsersOverCertainAgeAsync(certainAge);
    }

    public Task CreateUserAsync(string login, string password, string name, int gender,
        DateTime birthday, bool isAdmin, string createdBy)
    {
        LoggingData($"Creating user({login}) by params.");
        
        return _userService.CreateUserAsync(login, password, name, gender, birthday, isAdmin, createdBy);
    }

    public Task UpdateDataUserAsync(string login, string name, int gender, DateTime birthday, string modifiedBy)
    {
        LoggingData($"Updating data's user({login}).");

        return _userService.UpdateDataUserAsync(login, name, gender, birthday, modifiedBy);
    }

    public Task UpdateLoginUserAsync(string oldLogin, string newLogin, string modifiedBy)
    {
        LoggingData($"Updating login({newLogin}) user({oldLogin}).");

        return _userService.UpdateLoginUserAsync(oldLogin, newLogin, modifiedBy);
    }

    public Task UpdatePasswordUserAsync(string login, string newPassword, string modifiedBy)
    {
        LoggingData($"Updating password user({newPassword}).");

        return _userService.UpdatePasswordUserAsync(login, newPassword, modifiedBy);
    }

    public Task SoftDeleteUser(string login, string revokedBy)
    {
        LoggingData($"Soft deleting user({login}).");

        return _userService.SoftDeleteUser(login, revokedBy);
    }

    public Task HardDeleteUser(string login)
    {
        LoggingData($"Hard deleting user({login}).");

        return _userService.HardDeleteUser(login);
    }

    public Task RecoveryUser(string login)
    {
        LoggingData($"Recovering user({login}).");

        return _userService.RecoveryUser(login);
    }

    public Task<bool> UserConfirmationAsync(string login, string password)
    {
        LoggingData($"Action performed by user({login}).");
        
        return _userService.UserConfirmationAsync(login, password);
    }

    public Task<bool> AdminConfirmationAsync(string login, string password)
    {
        LoggingData($"Action performed by admin({login}).");
        
        return _userService.AdminConfirmationAsync(login, password);
    }
}