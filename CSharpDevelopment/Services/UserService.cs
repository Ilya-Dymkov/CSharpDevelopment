using CSharpDevelopment.Data;
using CSharpDevelopment.Models;
using CSharpDevelopment.Services.SourcesService;
using Microsoft.EntityFrameworkCore;

namespace CSharpDevelopment.Services;   

public class UserService : IUserService
{
    private readonly DataDbContext _context;

    public UserService() => _context = new DataDbContext();

    public UserService(Func<DbContextOptionsBuilder, DbContextOptionsBuilder> func) => 
        _context = new DataDbContext(func);

    public Task<List<User>> GetActiveUsersAsync() =>
        _context.Users
            .Where(u => u.RevokedOn == null)
            .OrderBy(u => u.CreatedOn)
            .ToListAsync();

    public Task<User?> GetUserAsync(string login) =>
        _context.Users.FirstOrDefaultAsync(u => u.Login == login);

    public Task<List<User>> GetUsersOverCertainAgeAsync(uint certainAge) =>
        _context.Users
            .Where(u => (DateTime.Now - u.Birthday).Days > certainAge * 365.25)
            .ToListAsync();

    public async Task CreateUserAsync(string login, string password, string name, int gender,
        DateTime birthday, bool isAdmin, string createdBy)
    {   
        await _context.Users.AddAsync(new User().GetNewUser(_context.Users, login, password, name, gender,
            birthday, isAdmin, createdBy));
        await _context.SaveChangesAsync();
    }

    private async Task BaseOfActionOnUser(string login, Action<User> action)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
        
        if (user == null)
            throw new ArgumentException("There is no such login!");

        action(user);

        await _context.SaveChangesAsync();
    }
    
    private async Task BaseOfUpdateAsync(string login, Action<User> actionUpdate, string modifiedBy) =>
        await BaseOfActionOnUser(login, user =>
        {
            actionUpdate(user);
            user.SetModified(DateTime.Now, modifiedBy);
        });

    public async Task UpdateDataUserAsync(string login, string name, int gender, DateTime birthday, string modifiedBy) =>
        await BaseOfUpdateAsync(login, user =>
        {
            user.SetName(name);
            user.SetGender(gender);
            user.SetBirthday(birthday);
        }, modifiedBy);

    public async Task UpdateLoginUserAsync(string oldLogin, string newLogin, string modifiedBy) =>
        await BaseOfUpdateAsync(oldLogin, user => user.SetLogin(newLogin, _context.Users), modifiedBy);

    public async Task UpdatePasswordUserAsync(string login, string newPassword, string modifiedBy) => 
        await BaseOfUpdateAsync(login, user => user.SetPassword(newPassword), modifiedBy);

    public async Task SoftDeleteUser(string login, string revokedBy) =>
        await BaseOfActionOnUser(login, user => user.SetRevoked(DateTime.Now, revokedBy));

    public async Task HardDeleteUser(string login) =>
        await BaseOfActionOnUser(login, user => _context.Users.Remove(user));

    public async Task RecoveryUser(string login) =>
        await BaseOfActionOnUser(login, user => user.SetRevoked(null, null));

    public Task<bool> UserConfirmationAsync(string login, string password) =>
        Task.FromResult(_context.Users.Any(u =>
            u.Login == login && u.Password == User.GetHashPassword(password) && u.RevokedOn == null));

    public Task<bool> AdminConfirmationAsync(string login, string password) =>
        Task.FromResult(_context.Users.Any(u =>
            u.Login == login && u.Password == User.GetHashPassword(password) && u.RevokedOn == null && u.IsAdmin));
}