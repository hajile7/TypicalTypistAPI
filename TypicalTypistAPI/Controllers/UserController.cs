using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TypicalTypistAPI.Models;
using Microsoft.EntityFrameworkCore;
using TypicalTypistAPI.Services;


namespace TypicalTypistAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(TypicalTypistDbContext context, Uploader uploader, PasswordService passwordService) : ControllerBase
    {
        private readonly TypicalTypistDbContext dbContext = context;

        private readonly Uploader uploader = uploader;

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
                Image = u.Image != null ? convertImageDTO(u.Image) : new ImageDTO { ImagePath = "Images/DefaultProfPic/V1DefaultProfilePic.webp" },
            };
        }

        static ImageDTO? convertImageDTO(Image i)
        {
            if (i == null)
            {
                return null;
            }

            return new ImageDTO
            {
                ImageId = i.ImageId,
                ImagePath = i.ImagePath
            };
        }

        // API Calls

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            User? result = await dbContext.Users.Include(i => i.Image).Where(u => u.Active == true).FirstOrDefaultAsync(u => u.UserId == userId);

            if (result == null || result.Active == false)
            {
                return NotFound("User not found");
            }

            return Ok(convertUserDTO(result));
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            User? result = await dbContext.Users.Include(i => i.Image).Where(u => u.Active == true).FirstOrDefaultAsync(u => u.UserName == username);

            if (result == null || result.Active == false)
            {
                return NotFound("User not found");
            }

            bool isPasswordValid = passwordService.VerifyPassword(password, result.Password);

            if (!isPasswordValid)
            {
                return Unauthorized("Invalid Password");
            }
            return Ok(convertUserDTO(result));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromForm] PostUserDTO u)
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

            if (u.Image != null)
            {
                Image? newImage = await uploader.GetImageAsync(u.Image, "Users");
                if (newImage != null)
                {
                    dbContext.Images.Add(newImage);
                    await dbContext.SaveChangesAsync();
                    newUser.ImageId = newImage.ImageId;
                }
            }
            else
            {
                newUser.ImageId = 1;
            }

            dbContext.Users.Add(newUser);
            await dbContext.SaveChangesAsync();

            return Ok(convertUserDTO(newUser));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromForm] PutUserDTO u, int id)
        {
            User? updateUser = await dbContext.Users.Include(i => i.Image).FirstOrDefaultAsync(user => user.UserId == id);

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

            if (u.Image != null)
            {
                Image? newImage = await uploader.GetImageAsync(u.Image, "Users");
                if (newImage != null)
                {
                    if (updateUser.Image != null &&
                        System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), updateUser.Image.ImagePath)) &&
                        updateUser.Image.ImageId != 1) 
                    {
                        try
                        {
                            System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), updateUser.Image.ImagePath));
                            dbContext.Images.Remove(updateUser.Image);
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, "Error deleting the old image: " + ex.Message);
                        }
                    }

                    updateUser.ImageId = newImage.ImageId;
                    updateUser.Image = newImage;
                }
            }

            if (updateUser.ImageId == null)
            {
                updateUser.ImageId = 1;
                updateUser.Image = await dbContext.Images.FindAsync(1);
            }

            dbContext.Users.Update(updateUser);
            await dbContext.SaveChangesAsync();

            return Ok(convertUserDTO(updateUser));

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            User? result = await dbContext.Users.Include(i => i.Image).FirstOrDefaultAsync(u => u.UserId == id);

            if (result == null || result.Active == false)
            {
                return NotFound("User Not Found");
            }

            result.Active = false;

            if (result.Image != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), result.Image.ImagePath)) &&
            result.Image.ImageId != 1)
            {
                try
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), result.Image.ImagePath));
                    dbContext.Images.Remove(result.Image);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Error deleting the image file: " + ex.Message);
                }
            }

            dbContext.Users.Update(result);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
