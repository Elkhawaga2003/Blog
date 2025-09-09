namespace Blog.Services.Interfaces
{
    public interface IFileServices
    {
        public bool Delete(string path);
        public Task<string> UploadAsync(IFormFile file, string location);
    }
}
