using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
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

            var authorizationHeader = http.Request.Headers["Authorization"].ToString();
            if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                // Now you have the token as a string
                if (!string.IsNullOrEmpty(token))
                {
                    var handler = new JwtSecurityTokenHandler();
                    if (handler.CanReadToken(token))
                    {
                        var jwtToken = handler.ReadJwtToken(token);

                        // Now you can access the token header and payload
                        var header = jwtToken.Header;
                        var claims = jwtToken.Claims;

                        int.TryParse(claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value, out userId);
                    }
                }
            }


            // int.TryParse(http.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value, out userId);
            this.User = this.users.FirstOrDefault(u => u.Id == userId);

            if (this.User == null) throw new InvalidCredentialException("User token is valid but user cannot be match!");

            this.IsAuthenticated = true;
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