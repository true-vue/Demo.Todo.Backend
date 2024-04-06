using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Data.Mockup;
using ToDoList.Model;

public class UserManager : IUserManager
{
    private MockupList<User> users;
    public UserManager(MockupList<User> users, IHttpContextAccessor httpContextAccessor)
    {
        var http = httpContextAccessor.HttpContext;
        this.users = users;
        if (http.User.Identity.IsAuthenticated)
        {
            var userId = 0;
            int.TryParse(http.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value, out userId);
            this.User = this.users.FirstOrDefault(u => u.Id == userId);
            this.IsAuthenticated = this.User == null ? false : true;
        }
    }

    public bool IsAuthenticated
    {
        get; private set;
    }

    public User? User
    {
        get; private set;
    } = null;
}