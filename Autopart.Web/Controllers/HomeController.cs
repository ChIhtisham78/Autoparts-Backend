using Autopart.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZikApp.API.Utilities;

namespace Autopart.Api.Controllers
{
    [Route("api/Home")]
    [Authorize]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ITempService _tempService;
        public HomeController(ITempService tempService)
        {
            _tempService = tempService;
        }
        [HttpGet("temps")]
        public async Task<ActionResult> GetTemps()
        {
            var userid = this.GetCurrentUserId();
            return Ok(await _tempService.GetTemps());
        }

        [HttpPost("temp")]
        public async Task<ActionResult> PostTemp(string name)
        {
            return Ok(await _tempService.AddTemp(name));
        }

        [HttpPut("temp")]
        public async Task<ActionResult> PutTemp(int id, string name)
        {
          var temp = await _tempService.UpdateTemp(id, name);
            if (temp == null)
            {
                return NotFound();
            }
            return Ok(temp);
        }
        [HttpGet("temp/{id}")]
        public async Task<ActionResult> GetTemp(int id)
        {
            var temp = await _tempService.GetTempById(id);
            if (temp == null)
            {
                return NotFound();
            }
            return Ok(temp);
        }
        [HttpDelete("temp/{id}")]
        public async Task<ActionResult> DeleteTemp(int id)
        {
            return Ok(await _tempService.RemoveTemp(id));
        }
    }
}
