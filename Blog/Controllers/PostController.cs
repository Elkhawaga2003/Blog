using System.Security.Claims;
using System.Threading.Tasks;
using Blog.ApiModels.Post;
using Blog.Models;
using Blog.Services.Implemetation;
using Blog.Services.Interfaces;
using Blog.UnitOfWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IToxicDetector _toxicDetector;
        private readonly UserManager<User> _userManager;
        private readonly IFileServices _fileServices;
        public PostController(IUnitOfWork unitOfWork, IFileServices fileServices, IToxicDetector toxicDetector, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _fileServices = fileServices;
            _userManager = userManager;
            _toxicDetector = toxicDetector;
        }

        [HttpGet]
        public async Task<ActionResult> GetPosts(int page = 1, int pageSize = 10)
        {
            var skip = (page - 1) * pageSize;

            var posts = _unitOfWork.Posts
                .Include(skip, pageSize, new List<string> 
                { "User", "Comments", "Comments.User","PostLikes","PostLikes.User" });

            var result = posts.Select(p => new
            {
                Id = p.Id,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                Likes = p.Likes,
                Image = p.ImageUrl,
                User = new
                {
                    Id = p.User.Id,
                    Name = p.User.Name,
                    Image = p.User.ImageUrl
                },
                LikedUserIds = p.PostLikes.Select(like => like.UserId),
                Comments = p.Comments.Select(c => new
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    User = new
                    {
                        Id = c.User.Id,
                        Name = c.User.Name,
                        Image = c.User.ImageUrl
                    }
                })
            });

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var post = await _unitOfWork.Posts.GetItemByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreatePost([FromForm] PostModel model)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null)
            {
                return Unauthorized("User not authenticated");
            }
            if (model == null)
            {
                return BadRequest("Post cannot be null");
            }
            if (_toxicDetector.IsToxic(model.Content))
            {
                return BadRequest("Post content is toxic");
            }
            var user = await _userManager.FindByEmailAsync(userEmail);
            var post = new Post
            {
                UserId = user.Id,
                Content = model.Content,
            };
            var img = await _fileServices.UploadAsync(model.Image, "/Posts/");
            if (img.StartsWith("/Posts/"))
            {
                post.ImageUrl = img;
            }

            await _unitOfWork.Posts.AddItemAsync(post);
            var result = _unitOfWork.Complete();
            if (result <= 0)
            {
                return BadRequest("Failed to create post");
            }
            ;
            return Ok(post);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("Like")]
        public async Task<ActionResult> LikePost(int postId)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null)
            {
                return Unauthorized("User not authenticated");
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            var post = await _unitOfWork.Posts.GetItemByIdAsync(postId);
            if (post == null)
            {
                return NotFound(new { error = "Post not found" });
            }

            var existingLike =  _unitOfWork.PostLikes.FindItem(
                pl => pl.PostId == postId && pl.UserId == user.Id);

            if (existingLike.Count()>=1)
            {
                return BadRequest(new { error = "You already liked this post" });
            }

            var postLike = new PostLike
            {
                PostId = postId,
                UserId = user.Id
            };

            await _unitOfWork.PostLikes.AddItemAsync(postLike);
            post.Likes++;

            _unitOfWork.Posts.UpdateItem(post);

            var result =  _unitOfWork.Complete();
            if (result <= 0)
            {
                return BadRequest(new { error = "Failed to like post" });
            }

            return Ok(new { message="added succesfuly"});
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("DisLike")]
        public async Task<ActionResult> DisLikePost(int postId)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null)
                return Unauthorized("User not authenticated");

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return NotFound(new { error = "User not found" });

            var post = await _unitOfWork.Posts.GetItemByIdAsync(postId);
            if (post == null)
                return NotFound(new { error = "Post not found" });

            // تحقق هل المستخدم عامل لايك أصلاً
            var existingLike = _unitOfWork.PostLikes.FindItem(
                pl => pl.PostId == postId && pl.UserId == user.Id
            ).FirstOrDefault();

            if (existingLike == null)
            {
                return BadRequest(new { error = "You have not liked this post yet" });
            }

            _unitOfWork.PostLikes.DeleteItem(existingLike);
            post.Likes--;

            _unitOfWork.Posts.UpdateItem(post);

            var result = _unitOfWork.Complete();
            if (result <= 0)
            {
                return BadRequest(new { error = "Failed to dislike post" });
            }

            return Ok(new { message = "Disliked successfully" });
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePost(int id)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null)
            {
                return Unauthorized("User not authenticated");
            }
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }
            var post = await _unitOfWork.Posts.GetItemByIdAsync(id);
            if (post == null)
            {
                return NotFound("Post not found");
            }
            if (post.UserId != user.Id)
            {
                return Forbid("You are not allowed to delete this post");
            }
            if (post.ImageUrl != null)
            {
                var fileName = post.ImageUrl;
                 _fileServices.Delete(fileName);
            }
            _unitOfWork.Posts.DeleteItem(post);
            var result = _unitOfWork.Complete();
            if (result <= 0)
            {
                return BadRequest(new { error = "Failed to delete post" });
            }
            return Ok(new {message= "Post deleted successfully" });
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("")]
        public async Task<ActionResult> Update(UpdatePost updatePost)
        {
            var post=await _unitOfWork.Posts.GetItemByIdAsync(updatePost.Id);
            if (post == null)
            {
                return NotFound("Post not found");
            }
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null)
            {
                return Unauthorized("User not authenticated");
            }
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }
            if (post.UserId != user.Id)
            {
                return Forbid("You are not allowed to update this post");
            }
            if (_toxicDetector.IsToxic(updatePost.Content))
            {
                return BadRequest("Post content is toxic");
            }
            post.Content = updatePost.Content;
            if (updatePost.Image != null)
            {
                if (post.ImageUrl != null)
                {
                    var fileName = post.ImageUrl;
                    _fileServices.Delete(fileName);
                }
                var img = await _fileServices.UploadAsync(updatePost.Image, "/Posts/");
                if (img.StartsWith("/Posts/"))
                {
                    post.ImageUrl = img;
                }
            }
            _unitOfWork.Posts.UpdateItem(post);
            var result = _unitOfWork.Complete();
            if (result <= 0)
            {
                return BadRequest("Failed to update post");
            }
            return Ok(post);
        }
    }
}
