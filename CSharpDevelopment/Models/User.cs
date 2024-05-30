using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace CSharpDevelopment.Models;

[PrimaryKey(nameof(System.Guid))]
public partial class User
{
    [JsonIgnore]
    public Guid Guid { get; private set; }

    public string Login { get; private set; }

    [JsonIgnore]
    public string Password { get; private set; }

    public string Name { get; private set; }
    public int Gender { get; private set; }
    public DateTime Birthday { get; private set; }

    [JsonIgnore]
    public bool IsAdmin { get; private set; }

    [JsonIgnore]
    public DateTime CreatedOn { get; private set; }

    [JsonIgnore]
    public string CreatedBy { get; private set; }

    [JsonIgnore]
    public DateTime ModifiedOn { get; private set; }

    [JsonIgnore]
    public string ModifiedBy { get; private set; }

    [JsonIgnore]
    public DateTime? RevorkedOn { get; private set; }
    [JsonIgnore]
    public string? RevorkedBy { get; private set; }
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

    public static string GetHashPassword(string newPassword) => 
        BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes(newPassword)))
            .Replace("-", "")
            .ToLower();

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

    public void SetGender(int gender)
    {
        if (gender is < 0 or > 2)
            throw new ArgumentException("Gender is not set correctly!");

        Gender = gender;
    }

    public void SetBirthday(DateTime birthday) => Birthday = birthday;

    public void SetModified(DateTime modifiedOn, string modifiedBy)
    {
        ModifiedOn = modifiedOn;
        ModifiedBy = modifiedBy;
    }

    public void SetRevorked(DateTime? revorkedOn, string? revorkedBy)
    {
        RevorkedOn = revorkedOn;
        RevorkedBy = revorkedBy;
    }

    public User GetNewUser(DbSet<User> users, string login, string password, string name, int gender,
        DateTime birthday, bool isAdmin, string createdBy)
    {
        Guid = Guid.NewGuid();
        SetLogin(login, users);
        SetPassword(password);
        SetName(name);
        SetGender(gender);
        SetBirthday(birthday);
        IsAdmin = isAdmin;
        CreatedOn = DateTime.Now;
        CreatedBy = createdBy;
        SetModified(DateTime.Now, createdBy);

        return this;
    }
}