using Blog.Services.Interfaces;

namespace Blog.Services.Implemetation
{
    public class FileServices : IFileServices
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileServices(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public bool Delete(string path)
        {
            var deriction = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + path);
            if (File.Exists(deriction))
            {
                File.Delete(deriction);
                return true;
            }
            return false;
        }

        public async Task<string> UploadAsync(IFormFile file, string location)
        {

            try
            {
                if (file != null && file.Length > 0)
                {
                    var path = _webHostEnvironment.WebRootPath + location;
                    var extension = Path.GetExtension(file.FileName);//.jpg
                    var filename = Guid.NewGuid().ToString().Replace("-", string.Empty) + extension;
                    if (!File.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (FileStream filestream = File.Create(path + filename))
                    {
                        await file.CopyToAsync(filestream);
                        filestream.Flush();
                        return $"{location}{filename}";
                    }

                }
                else
                {
                    return "empty";
                }
            }
            catch (Exception ex)
            {
                return ex.Message + "--" + ex.InnerException;
            }
        }
    }
}
