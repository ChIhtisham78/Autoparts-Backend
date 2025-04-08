using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("Analytics")]
        public async Task<IActionResult> GetAnalytics(int? vendorId = null)
        
        {
            var summary = await _analyticsService.GetAnalyticsSummary(vendorId);
            if (summary == null)
            {
               

                return Ok(new
                {
                   
                });
            }
            return Ok(summary);
        }
    }
}
