using TypicalTypistAPI.Models;

namespace TypicalTypistAPI.Classes
{
    public class Uploader
    {
        private TypicalTypistDbContext dbContext = new TypicalTypistDbContext();
        public Image getImage(IFormFile file, string folderName)
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

            using FileStream stream = new FileStream(Path.Combine(filePath, fileName), FileMode.Create);
            file.CopyTo(stream);

            Image newImage = new Image()
            {
                ImageId = 0,
                ImagePath = Path.Combine($"Images\\{folderName}", fileName)
            };

            dbContext.Images.Add(newImage);
            dbContext.SaveChanges();

            return newImage;
        }
    }
}
