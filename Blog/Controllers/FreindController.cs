using System.Security.Claims;
using System.Threading.Tasks;
using Blog.ApiModels.Friend;
using Blog.Helpers;
using Blog.Models;
using Blog.UnitOfWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class FreindController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private IHubContext<NotificationHub> _hubContext;
        public FreindController(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _userManager = userManager;
        }
        [HttpPost("send-request")]
        public async Task<ActionResult> SendRequest(FriendReqDTO friendReqDTO)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null)
            {
                return BadRequest(new { error = "User not found" });
            }
            var user = await _userManager.FindByEmailAsync(userEmail);
            var items = _unitOfWork.FriendShips.FindItem(f => (f.RequesterId == user.Id && f.AccepterId == friendReqDTO.AccepterId) || (f.RequesterId == friendReqDTO.AccepterId && f.AccepterId == user.Id));

            if (items.Count() > 0)
            {
                return BadRequest(new { error = "Friend request already exists" });
            }
            var friendShip = new FriendShip
            {
                RequesterId = user.Id,
                AccepterId = friendReqDTO.AccepterId
                ,
                Status = FriendShipStatus.Pending
            };
            await _unitOfWork.FriendShips.AddItemAsync(friendShip);
            var saveResult = _unitOfWork.Complete();
            if (saveResult <= 0)
            {
                return BadRequest(new { error = "failed to send reqeust" });
            }
            await _hubContext.Clients
                  .All
                  .SendAsync("ReceiveNotification", new
                  {
                      Type = "FriendRequest",
                      FromUserName = user.UserName,
                      image = user.ImageUrl,
                      Message = $"{user.UserName} sent a friend request "
                  });
            return Ok(new { message = "Friend request sent successfully" });
        }
        [HttpPost("accept-request")]
        public async Task<ActionResult> AcceptFrind(AcceptDto acceptDto)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null)
            {
                return BadRequest(new { error = "User not found" });
            }
            var user = _userManager.FindByEmailAsync(userEmail).Result;
            var items = _unitOfWork.FriendShips.FindItem(f => f.AccepterId == user.Id && f.Id == acceptDto.Id);
            var friendShip = items.FirstOrDefault();
            if (friendShip == null)
            {
                return BadRequest(new { error = "No pending friend requests found" });
            }
            friendShip.Status = FriendShipStatus.Accepted;
            _unitOfWork.FriendShips.UpdateItem(friendShip);
            var saveResult = _unitOfWork.Complete();
            if (saveResult <= 0)
            {
                return BadRequest(new { error = "Failed to accept friend request" });
            }
            await _hubContext.Clients
        .All
        .SendAsync("ReceiveNotification", new
        {
            Type = "AcceptRequest",
            FromUserName = user.UserName,
            image = user.ImageUrl,

            Message = $"{user.UserName} accepted your friend request."
        });
            return Ok(new { message = "Friend request accepted successfully" });
        }
        [HttpPost("reject-request")]
        public async Task<ActionResult> RejectFrind(AcceptDto acceptDto)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null)
            {
                return BadRequest(new { error = "User not found" });
            }
            var user = await _userManager.FindByEmailAsync(userEmail);
            var items = _unitOfWork.FriendShips.FindItem(f => f.AccepterId == user.Id && f.Id == acceptDto.Id);
            var friendShip = items.FirstOrDefault();
            if (friendShip == null)
            {
                return BadRequest(new { error = "No pending friend requests found" });
            }
            friendShip.Status = FriendShipStatus.Rejected;
            _unitOfWork.FriendShips.UpdateItem(friendShip);
            var saveResult = _unitOfWork.Complete();
            if (saveResult <= 0)
            {
                return BadRequest(new { error = "Failed to reject friend request" });
            }
            return Ok(new { message = "Friend request rejected successfully" });
        }
        [HttpGet("friend-requests")]
        public async Task<ActionResult> GetFriendRequests()
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null)
            {
                return BadRequest(new { error = "User not found" });
            }
            var user = await _userManager.FindByEmailAsync(userEmail);

            var friendRequests = _unitOfWork.FriendShips.FindItems(
                f => f.AccepterId == user.Id && f.Status == FriendShipStatus.Pending,
                new List<string> { "Requester" }
            )
            .Select(f => new
            {
                RequestId = f.Id,
                RequesterId = f.RequesterId,
                RequesterName = f.Requester.Name,
                Image = f.Requester.ImageUrl,
                RequestedAt = f.CreatedAt
            });

           
            return Ok(friendRequests);
        }

        [HttpGet("list-users")]
        public async Task<ActionResult> GetUsers()
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null)
            {
                return BadRequest(new { error = "User not found" });
            }
            var user = await _userManager.FindByEmailAsync(userEmail);
            var usersList = await _userManager.Users
             .Where(u => u.Id != user.Id)
             .Select(u => new { u.Id, u.Name, u.ImageUrl })
             .ToListAsync();

            if (!usersList.Any())
            {
                return NotFound(new { message = "No users found" });
            }

            return Ok(usersList);

        }

        [HttpGet("list-friends")]
        public async Task<ActionResult> GetFriends()
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userEmail == null)
            {
                return BadRequest(new { error = "User not found" });
            }
            var user = await _userManager.FindByEmailAsync(userEmail);
            var friendRequests = _unitOfWork.FriendShips.FindItems(
                f => (f.RequesterId == user.Id || f.AccepterId == user.Id) && f.Status == FriendShipStatus.Accepted,
                new List<string> { "Requester", "Accepter" }
            )
            .Select(f => new
            {
                FriendShipId = f.Id,
                FriendId = user.Id == f.RequesterId ? f.AccepterId : f.RequesterId,
                FriendName = user.Id == f.RequesterId ? f.Accepter.Name : f.Requester.Name,
                FriendImage = user.Id == f.RequesterId ? f.Accepter.ImageUrl : f.Requester.ImageUrl,
                AcceptedAt = f.CreatedAt
            });

            if (!friendRequests.Any())
            {
                return NotFound(new { message = "No friend requests found" });
            }

            return Ok(friendRequests);
        }
    }
}
