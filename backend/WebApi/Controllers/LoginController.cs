using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        [HttpGet("Test")]
        public async Task<IActionResult> Test()
        {
            return Ok(new
            {
                message = $"Dummy test ",
                url = "Google.com",
                JWTToken = "",
                Status = true
            });
        }

    }
}
