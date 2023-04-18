using JWTDemo.Auth;
using JWTDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace JWTDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwtController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        public JwtController(ApplicationDbContext db,IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }
        [HttpPost]
        [Route("UserRegistration")]
        public async Task<ActionResult<UserTbl>> Register(UserTbl? u)
        {
            await _db.Users.AddAsync(u);
            await _db.SaveChangesAsync();
            return Ok(u); 
        }
        [HttpPost]
        [Route("UserLogin")]
        public async Task<ActionResult<JWT>> Login(UserTbl u)
        {
            JWT jwt = new JWT();
            try
            {
                var user = (from i in _db.Users
                            where i.mail == u.mail && i.password == u.password
                            select i).SingleOrDefault();
                if (user != null)
                {
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,user.mail),
                        new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    };
                    var token = GetToken(authClaims);
                    string s = new JwtSecurityTokenHandler().WriteToken(token);
                    jwt.User = user;
                    jwt.key = s;
                    return jwt;
                }
                else
                {
                    return null;
                }
                return Unauthorized();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Wrong Entry!...");
            }
        }
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(5),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }

    }
        
}
