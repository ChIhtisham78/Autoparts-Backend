using System.Threading.Tasks;
using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomePageController : ControllerBase
    {
        private readonly IHomePageService _homePageService;

        public HomePageController(IHomePageService homePageService)
        {
            _homePageService = homePageService;
        }

        [HttpGet("GetAllPages")]
        public async Task<IActionResult> GetHomePages()
        {
            var homePages = await _homePageService.GetHomePagesAsync();
            return Ok(homePages);
        }

        [HttpGet("GetAllPages/{id}")]
        public async Task<IActionResult> GetHomePageById(int id)
        {
            var homePage = await _homePageService.GetHomePageByIdAsync(id);
            if (homePage == null)
            {
                return NotFound();
            }
            return Ok(homePage);
        }

        [HttpPut("UpdatePages/{id}")]
        public async Task<IActionResult> UpdateHomePage(int id, [FromBody] HomePageDto homePageDto)
        {

           var updatedHomePage = await _homePageService.UpdateHomePageAsync(id, homePageDto);
           return Ok(updatedHomePage);
            
        }

        [HttpDelete("DeletePages/{id}")]
        public async Task<IActionResult> DeleteHomePage(int id)
        {
            try
            {
                await _homePageService.RemoveHomePageAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
