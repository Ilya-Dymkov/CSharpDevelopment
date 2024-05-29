using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace CSharpDevelopment.Models;

[PrimaryKey(nameof(System.Guid))]
public partial class User(
    string login,
    string password,
    string name,
    int gender,
    DateTime birthday,
    bool isAdmin,
    string createdBy)
{
    [JsonIgnore]
    public Guid Guid { get; set; } = Guid.NewGuid();

    public string Login { get; private set; } = login;

    [JsonIgnore]
    public string Password { get; private set; } = GetHashPassword(password);

    public string Name { get; private set; } = name;
    public int Gender { get; set; } = gender;
    public DateTime Birthday { get; set; } = birthday;

    [JsonIgnore]
    public bool IsAdmin { get; set; } = isAdmin;

    [JsonIgnore]
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    [JsonIgnore]
    public string CreatedBy { get; set; } = createdBy;

    [JsonIgnore]
    public DateTime ModifiedOn { get; set; } = DateTime.Now;

    [JsonIgnore]
    public string ModifiedBy { get; set; } = createdBy;

    [JsonIgnore]
    public DateTime? RevorkedOn { get; set; }
    [JsonIgnore]
    public string? RevorkedBy { get; set; }
    public string IsActive => RevorkedOn == null ? "Active" : "Inactive";
    
    [GeneratedRegex("^[a-zA-Z0-9]+$")]
    private static partial Regex LettersAndNumbersRegex();

    public void SetLogin(string login, DbSet<User> users)
    {
        if (users.Any(u => u.Login == login))
            throw new ArgumentException("This login is already occupied!");

        if (!LettersAndNumbersRegex().IsMatch(login))
            throw new ArgumentException("Username is not set correctly!");

        Login = login;
    }
    
    public static string GetHashPassword(string newPassword) => newPassword;

    public void SetPassword(string newPassword)
    {
        if (!LettersAndNumbersRegex().IsMatch(newPassword))
            throw new ArgumentException("Password is not set correctly!");

        Password = GetHashPassword(newPassword);
    }

    [GeneratedRegex("^[a-zA-Zа-яА-Я]+$")]
    private static partial Regex EnAndRuLettersRegex();

    public void SetName(string name)
    {
        if (!EnAndRuLettersRegex().IsMatch(name))
            throw new ArgumentException("Name is not set correctly!");

        Name = name;
    }
}