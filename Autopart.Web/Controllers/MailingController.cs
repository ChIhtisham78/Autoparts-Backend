using Autopart.API.Infrastructure;
using Autopart.Application.Models;
using Autopart.Infrastructure.SendGrid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Autopart.Infrastructure.SendGrid.SendGridSetting;

namespace Autopart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MailingController : ControllerBase
    {
        private readonly SendGridSetting _sendGridSetting;
        public MailingController(SendGridSetting sendGridSetting)
        {
            _sendGridSetting = sendGridSetting;
        }
        [HttpPost("sendemail")]
        public async Task<IActionResult> SendEmail(string to,string body,string subject,List<AttachmentModel> attachments) 
        {
            var res = await _sendGridSetting.EmailAsync(subject, to, "", "", body, attachments);
            if (res)
            {
                return Ok("Email Sent Sucessfully!!");
            }
            else
            {
                return BadRequest("Email Not Sent Sucessfully!!");
            }
            
        }
    }
}
