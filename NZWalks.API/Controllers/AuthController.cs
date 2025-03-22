using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Username
            };

            var identityResult = await userManager.CreateAsync(identityUser, registerDto.Password);

            if (identityResult.Succeeded)
            {
                if (registerDto.Roles != null && registerDto.Roles.Any())
                    identityResult = await userManager.AddToRolesAsync(identityUser, registerDto.Roles);

                if (identityResult.Succeeded)
                {
                    return Ok("User has registerd successfully! Please login");
                }
                
            }
            return BadRequest("Something went wrong");
            
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Username);
            if(user != null)
            {
                var checkPassword = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);

                if (checkPassword)
                { 
                    //get the user roles
                    var roles = await userManager.GetRolesAsync(user);

                    //Create Token
                    if (roles != null)
                    {
                        var token = tokenRepository.CreateJWTToken(user, roles.ToList());
                        return Ok(new LoginResponseDto { JwtToken = token });
                    }
                }
            }

            return BadRequest("Username or password is wrong.");
        }
    }
}
