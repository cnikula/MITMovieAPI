using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MoveAPI.Models;
using MoveAPI.Services.Interfaces;

namespace MoveAPI.Services
{
    public class UserService : IUserService
    {
        private List<User> _users = new List<User>
        {
            new User { UserName = "nick", Password = "@Test123" }

        };

        private readonly IConfiguration _configuration;

        public UserService(IConfiguration configuration)
        {
                _configuration = configuration;
        }

        
        public string Login(User user)
        {
            {
                var LoginUser = _users.SingleOrDefault(x => x.UserName == user.UserName && x.Password == user.Password);

                if (LoginUser == null)
                {
                    return string.Empty;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.UserName)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                string userToken = tokenHandler.WriteToken(token);
                return userToken;
            }
        }


    }
}
