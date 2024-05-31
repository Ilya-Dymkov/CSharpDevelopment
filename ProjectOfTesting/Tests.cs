using CSharpDevelopment.Proxies;
using CSharpDevelopment.Services.SourcesService;
using Microsoft.EntityFrameworkCore;

namespace ProjectOfTesting;

public class Tests
{
    private readonly IUserService _userService = new UserServiceProxy(builder => 
        builder.UseSqlite(connectionString:
            @"Data Source=C:\Users\Ilya\RiderProjects\CSharpDevelopment\ProjectOfTesting\DataBases\testing.db"));

    [Test]
    public void Test1() => Assert.That(_userService.GetUserAsync("root").Result?.Name, Is.EqualTo("root"));
}