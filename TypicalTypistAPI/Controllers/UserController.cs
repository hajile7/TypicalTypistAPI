using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TypicalTypistAPI.Models;
using Microsoft.EntityFrameworkCore;
using TypicalTypistAPI.Services;


namespace TypicalTypistAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(TypicalTypistDbContext context, PasswordService passwordService) : ControllerBase
    {
        private readonly TypicalTypistDbContext dbContext = context;

        private readonly PasswordService passwordService = passwordService;

        // DTO Conversions
        static UserDTO convertUserDTO(User u)
        {
            return new UserDTO
            {
                UserId = u.UserId,
                Joined = u.Joined,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                Email = u.Email,
            };
        }

        // API Calls

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            User? result = await dbContext.Users.Where(u => u.Active == true).FirstOrDefaultAsync(u => u.UserId == userId);

            if (result == null || result.Active == false)
            {
                return NotFound("User not found");
            }

            return Ok(convertUserDTO(result));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            User? result = await dbContext.Users.Where(u => u.Active == true).FirstOrDefaultAsync(u => u.UserName == loginModel.Username);

            if (result == null || result.Active == false)
            {
                return NotFound("User not found");
            }

            bool isPasswordValid = passwordService.VerifyPassword(loginModel.Password, result.Password);

            if (!isPasswordValid)
            {
                return Unauthorized("Invalid Password");
            }
            return Ok(convertUserDTO(result));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(PostUserDTO u)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await dbContext.Users.AnyAsync(a => a.UserName == u.UserName))
            {
                return BadRequest(u.UserName + " is already in use");
            }

            User newUser = new User
            {
                UserName = u.UserName,
                Password = passwordService.HashPassword(u.Password),
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            };

            dbContext.Users.Add(newUser);
            await dbContext.SaveChangesAsync();

            return Ok(convertUserDTO(newUser));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(PutUserDTO u, int id)
        {
            User? updateUser = await dbContext.Users.FirstOrDefaultAsync(user => user.UserId == id);

            if (updateUser == null || updateUser.Active == false)
            {
                return NotFound("User Not Found");
            }

            if (u.FirstName != null) updateUser.FirstName = u.FirstName;
            if (u.LastName != null) updateUser.LastName = u.LastName;
            if (u.UserName != null)
            {
                if (await dbContext.Users.AnyAsync(o => o.UserName == u.UserName && u.UserName != updateUser.UserName))
                {
                    return BadRequest("Username is already in use.");
                }
                updateUser.UserName = u.UserName;
            }
            if (u.Email != null)
            {
                if (await dbContext.Users.AnyAsync(o => o.Email == u.Email && u.Email != updateUser.Email && o.Active == true))
                {
                    return BadRequest("Email is already in use.");
                }
                updateUser.Email = u.Email;
            }

            dbContext.Users.Update(updateUser);
            await dbContext.SaveChangesAsync();

            return Ok(convertUserDTO(updateUser));

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            User? result = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == id);

            if (result == null || result.Active == false)
            {
                return NotFound("User Not Found");
            }

            result.Active = false;

            dbContext.Users.Update(result);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
