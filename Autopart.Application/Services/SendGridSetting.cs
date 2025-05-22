using Autopart.Application.Models;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace Autopart.Application.Services
{
    public class SendGridSetting
    {
        private readonly SendGridSection _sendGridSection;
        public SendGridSetting(IOptions<SendGridSection> sendGridSection)
        {
            _sendGridSection = sendGridSection.Value;
        }
        public async Task<bool> EmailAsync(string subject, string email, string userName, string message1, string body, List<AttachmentModel> attachments)
        {
            var client = new SendGridClient(_sendGridSection.ApiKey);
            var from = new EmailAddress(_sendGridSection.From, _sendGridSection.UserName);
            var to = new EmailAddress(email, userName);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message1, body);

            if (attachments != null && attachments.Count > 0)
            {
                foreach (var attachmentModel in attachments)
                {
                    var attachment = new Attachment
                    {
                        Content = attachmentModel.FilePath,
                        Filename = attachmentModel.FileName,
                        Type = attachmentModel.MimeType,
                        Disposition = "attachment"
                    };
                    msg.AddAttachment(attachment);
                }
            }

            var responce = await client.SendEmailAsync(msg);
            return responce.IsSuccessStatusCode;
        }

    }
}