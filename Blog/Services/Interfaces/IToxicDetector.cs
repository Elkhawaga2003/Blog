namespace Blog.Services.Interfaces
{
    public interface IToxicDetector
    {
        public bool IsToxic(string content);
    }
}
