using DAL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Dtos.AuthDtos;
using WebAPI.Security;

namespace WebAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepo;

        public AuthController(IConfiguration config, IUserRepository userRepo)
        {
            _config = config;
            _userRepo = userRepo;
        }

        [HttpPost("register")]
        public ActionResult<UserRegisterDto> Register([FromBody] UserRegisterDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var trimmedUsername = userDto.Username.Trim();

                if (_userRepo.UsernameExists(trimmedUsername))
                    return BadRequest($"Username {trimmedUsername} already exists");

                var b64salt = PasswordHashProvider.GetSalt();
                var b64hash = PasswordHashProvider.GetHash(userDto.Password, b64salt);

                var user = new User
                {
                    Username = trimmedUsername,
                    PwdHash = b64hash,
                    PwdSalt = b64salt,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Email = userDto.Email,
                    Phone = userDto.Phone,
                    RoleId = 2
                };

                _userRepo.Create(user);

                return Ok(userDto);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] UserLoginDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var genericLoginFail = "Invalid credentials";

                var trimmedUsername = dto.Username.Trim();

                var existingUser = _userRepo.RetrieveByUsername(trimmedUsername);

                if (existingUser == null)
                    return BadRequest(genericLoginFail);

                var hash = PasswordHashProvider.GetHash(dto.Password, existingUser.PwdSalt);

                if (hash != existingUser.PwdHash)
                    return BadRequest(genericLoginFail);

                var secureKey = _config["JWT:SecureKey"];

                if (string.IsNullOrEmpty(secureKey))
                    return StatusCode(500, "JWT secure key is missing.");

                var token = JwtTokenProvider.CreateToken(
                    secureKey,
                    120,
                    existingUser.Username,
                    existingUser.Role.Name
                );

                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("changepassword")]
        public ActionResult ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var username = User.Identity!.Name!;

                var user = _userRepo.RetrieveByUsername(username);

                if (user == null)
                    return NotFound("User not found.");

                var currentHash = PasswordHashProvider.GetHash(dto.CurrentPassword, user.PwdSalt);

                if (currentHash != user.PwdHash)
                    return BadRequest("Current password is incorrect");

                var newSalt = PasswordHashProvider.GetSalt();
                var newHash = PasswordHashProvider.GetHash(dto.NewPassword, newSalt);

                user.PwdSalt = newSalt;
                user.PwdHash = newHash;

                _userRepo.Update(user);

                return Ok("Password changed successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}

