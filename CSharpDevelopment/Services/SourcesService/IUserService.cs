using CSharpDevelopment.Models;

namespace CSharpDevelopment.Services.SourcesService;

public interface IUserService
{
    Task<List<User>> GetActiveUsersAsync();
    Task<User?> GetUserAsync(string login);
    Task<List<User>> GetUsersOverCertainAgeAsync(DateTime certainAge);
    Task CreateUserAsync(string login, string password, string name, int gender, DateTime birthday, 
        bool isAdmin, string createdBy);
    Task UpdateDataUserAsync(string login, string name, int gender, DateTime birthday, string modifiedBy);
    Task UpdateLoginUserAsync(string oldLogin, string newLogin, string modifiedBy);
    Task UpdatePasswordUserAsync(string login, string newPassword, string modifiedBy);
    Task SoftDeleteUser(string login, string revorkedBy);
    Task HardDeleteUser(string login);
    Task RecoveryUser(string login);
    Task<bool> UserConfirmationAsync(string login, string password);
    Task<bool> AdminConfirmationAsync(string login, string password);
}