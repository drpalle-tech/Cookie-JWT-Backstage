using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UnderTheHoodWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]
        public IActionResult Authenticate([FromBody] Credential credential)
        {
            if (credential.UserName == "s" && credential.Password == "s")
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, credential.UserName),
                    new Claim("admin", "true"),
                    new Claim("DOJ", "2021-01-01")
                };
                var expiresAt = DateTime.UtcNow.AddMinutes(10);

                
                //JWT token contains XXX.YYY.ZZZ
                //XXX is the hashing algorithm that contains forula to decrypt JSON
                //YYY is the actual claim data list.
                //ZZZ is the hashed claim after the hash algo does it's magic.
                //Hashing algo needs a security key to create the hashed claims.
                //This security key is provided along with JWT.

                //On creation the above process is done.
                //on every request, the hashing algorithm does process and generates hashed claims(ZZZ) and compares with original JWT for equality to validate.

                var token = CreateJwt(claims, expiresAt);

                return Ok(new
                {
                    access_token = token,
                    expires_at = expiresAt
                });
            }

            ModelState.AddModelError("Unauthorized", "Hey Black Sheep!!! Meh... Meh...");
            return Unauthorized(ModelState);
        }

        private string CreateJwt(IEnumerable<Claim> claims, DateTime expires)
        {
            string securityKey = _configuration.GetValue<string>("secret") ?? string.Empty;
            var encodedKey = ASCIIEncoding.ASCII.GetBytes(securityKey);


            var jwt = new JwtSecurityToken(
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: expires,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(encodedKey), SecurityAlgorithms.HmacSha256Signature)
                    );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }

    public class Credential
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
