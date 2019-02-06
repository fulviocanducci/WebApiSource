using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApiSource.Models;

namespace WebApiSource.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public async Task<object> Post([FromBody] User user,
            [FromServices]UserManager<IdentityUser> userManager,
            [FromServices]SignInManager<IdentityUser> signInManager,
            [FromServices]SigningConfigurations signingConfigurations,
            [FromServices]TokenConfigurations tokenConfigurations)
        {
            await CreateUserOne(userManager);
            if (ModelState.IsValid)
            {
                var u = await userManager.FindByNameAsync(user.Email);
                var result = await signInManager.CheckPasswordSignInAsync(u, user.Password, false);
                if (result.Succeeded)
                {
                    ClaimsIdentity identity = new ClaimsIdentity(
                        new GenericIdentity(user.Email, "Login"),
                        new[] {
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                            new Claim(JwtRegisteredClaimNames.UniqueName, user.Email)
                        }
                    );
                    DateTime dataCriacao = DateTime.Now;
                    DateTime dataExpiracao = dataCriacao +
                        TimeSpan.FromSeconds(tokenConfigurations.Seconds);

                    var handler = new JwtSecurityTokenHandler();
                    var securityToken = handler.CreateToken(new SecurityTokenDescriptor
                    {
                        Issuer = tokenConfigurations.Issuer,
                        Audience = tokenConfigurations.Audience,
                        SigningCredentials = signingConfigurations.SigningCredentials,
                        Subject = identity,
                        NotBefore = dataCriacao,
                        Expires = dataExpiracao
                    });
                    var token = handler.WriteToken(securityToken);

                    return new
                    {
                        authenticated = true,
                        created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                        expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                        accessToken = token,
                        message = "OK"
                    };
                }
            }
            return new
            {
                authenticated = false,
                message = "Falha ao autenticar"
            };
        }

        private async Task CreateUserOne(UserManager<IdentityUser> userManager)
        {
            var result = await userManager.CreateAsync(new IdentityUser
            {
                Email = "fulviocanducci@hotmail.com",
                UserName = "fulviocanducci@hotmail.com",
                EmailConfirmed = true,                
            }, "Ab1059b@");
        }
    }
}