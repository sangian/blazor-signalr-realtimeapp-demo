using Backend.Authentication;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class AuthController(JwtTokenGenerator tokenGenerator) : ControllerBase
    {
        private readonly IReadOnlyDictionary<string, string> users = new Dictionary<string, string>
        {
            { "airplane-1", "password" },
            { "airplane-2", "password" },
            { "airplane-3", "password" },
            { "airplane-4", "password" },
            { "airplane-5", "password" },
            { "airplane-6", "password" },
            { "user1", "password" },
            { "user2", "password" },
            { "user3", "password" },
            { "user4", "password" },
            { "user5", "password" },
            { "user6", "password" },
        };


        [HttpPost("token", Name = nameof(GetAuthToken))]
        public ActionResult<TokenResponse> GetAuthToken([FromBody] TokenRequest request)
        {
            if (request is not null &&
                users.TryGetValue(request.Username.ToLowerInvariant(), out string? password) &&
                !string.IsNullOrWhiteSpace(request.Password) &&
                password.Equals(request.Password))
            {

                var token = tokenGenerator.GetJwtToken(request.Username, "Client");

                return Ok(new TokenResponse { AccessToken = token });
            }

            return Unauthorized();
        }
    }
}
