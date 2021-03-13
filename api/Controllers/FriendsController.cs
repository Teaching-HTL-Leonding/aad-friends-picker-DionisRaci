using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace ProtectedApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FriendsController : ControllerBase
    {
        public record Friend(int ID, string Customer, decimal Revenue);

        public readonly static ConcurrentBag<Friend> friends = new()
        {
            new(1, "Foo", 42m),
            new(2, "Bar", 84m)
        };

        [HttpGet]
        [RequiredScope("read")]
        [Authorize(Policy = "RaciOnly")]
        public IActionResult GetAllFriends()
        {
            Debug.WriteLine($"The user name is {User.Claims.First(c => c.Type == ClaimTypes.Name)}");
            Debug.WriteLine($"The AAD object ID for the user is {User.Claims.First(c => c.Type == ClaimConstants.ObjectId)}");
            return Ok(friends);
        }

        [HttpPost]
        [RequiredScope("write")]
        public IActionResult AddFriend([FromBody] Friend friend)
        {
            friends.Add(friend);
            return StatusCode((int)HttpStatusCode.Created);
        }

        [HttpPost("clear")]
        [RequiredScope("admin")]
        public IActionResult ClearFriends()
        {
            friends.Clear();
            return NoContent();
        }
    }
}
