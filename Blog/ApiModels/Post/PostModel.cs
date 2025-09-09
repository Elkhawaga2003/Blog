namespace Blog.ApiModels.Post
{
    public class PostModel
    {
        public string Content { get; set; }
        public IFormFile? Image { get; set; }
    }
}
