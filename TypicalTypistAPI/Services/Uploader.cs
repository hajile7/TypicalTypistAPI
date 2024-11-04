using Microsoft.EntityFrameworkCore;
using TypicalTypistAPI.Models;

namespace TypicalTypistAPI.Services
{
    public class Uploader
    {
        private readonly TypicalTypistDbContext dbContext;
        public Uploader(TypicalTypistDbContext context)
        {
            dbContext = context;
        }
        public async Task<Image?> GetImageAsync(IFormFile file, string folderName)
        {
            string[] validExtensions = [".jpg", ".png", ".jpeg", ".gif", ".webp", ".avif", ".svg", ".jfif", ".webp"];
            string fileExtension = Path.GetExtension(file.FileName);

            if (!validExtensions.Contains(fileExtension))
            {
                return null;
            }

            long size = file.Length;

            if (size > 1024 * 1024 * 5)
            {
                return null;
            }

            string fileName = Guid.NewGuid().ToString() + fileExtension;
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), $"Images\\{folderName}");

            try
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                using FileStream stream = new FileStream(Path.Combine(filePath, fileName), FileMode.Create);
                await file.CopyToAsync(stream);

                Image newImage = new Image()
                {
                    ImagePath = Path.Combine($"Images\\{folderName}", fileName)
                };

                await dbContext.Images.AddAsync(newImage);
                await dbContext.SaveChangesAsync();

                return newImage;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error while uploading the image: {ex.Message}");
                return null;
            }
        }
    }
}
