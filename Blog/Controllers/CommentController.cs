using System.Security.Claims;
using System.Threading.Tasks;
using Blog.ApiModels;
using Blog.ApiModels.Verfication;
using Blog.Models;
using Blog.Services.Interfaces;
using Blog.UnitOfWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CommentController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IToxicDetector _toxicDetector;
        private readonly UserManager<User> _userManager;
        public CommentController(IUnitOfWork unitOfWork,IToxicDetector toxicDetector,UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _toxicDetector = toxicDetector;
            _userManager = userManager;
        }
       
        [HttpPost("")]
        public async Task<ActionResult> AddComment(CommentModel commentModel)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userEmail == null)
            {
                return Unauthorized(new {error="you are not authorized"});
            }
            var user =await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }
            if (commentModel == null)
            {
                return BadRequest(new { error = "Comment cannot be null" });
            }
            if (_toxicDetector.IsToxic(commentModel.Content))
            {
                return BadRequest(new { error = "Comment content is toxic" });
            }
            var comment = new Comment
            {
                Content = commentModel.Content,
                PostId = commentModel.PostId,
                UserId = user.Id
            };
            await _unitOfWork.Comments.AddItemAsync(comment);
            var result =  _unitOfWork.Complete();
            if (result < 1)
            {
                return BadRequest(new { error = "Comment added successfully" });
            }
            return Ok(comment);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(UpdateComment updateComment)
        {
            var comment=await _unitOfWork.Comments.GetItemByIdAsync(updateComment.Id);
            if (comment == null)
            {
                return NotFound(new { error = "Comment not found" });
            }
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null)
            {
                return Unauthorized(new { error = "User not authenticated" });
            }
            if(comment.UserId != userEmail)
            {
                return Forbid(  "You are not allowed to update this comment" );
            }
            if (_toxicDetector.IsToxic(updateComment.Content))
            {
                return BadRequest(new { error = "Comment content is toxic" });
            }
            comment.Content = updateComment.Content;
             _unitOfWork.Comments.UpdateItem(comment);
            var result =_unitOfWork.Complete();
            if (result < 1)
            {
                return BadRequest(new { error = "Failed to update comment" });
            }
            return Ok(comment);

        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null)
            {
                return Unauthorized(new { error = "User not authenticated" });
            }
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }
            var comment = await _unitOfWork.Comments.GetItemByIdAsync(id);
            if (comment == null)
            {
                return NotFound(new { error = "Comment not found" });
            }
            if (comment.UserId != user.Id)
            {
                return Forbid( "You are not allowed to delete this comment" );
            }
            _unitOfWork.Comments.DeleteItem(comment);
            var result = _unitOfWork.Complete();
            if (result  <1)
            {
                return BadRequest(new { error = "Failed to delete comment" });
            }
            return Ok(new { message = "Comment deleted successfully" });
        }
    }
}
