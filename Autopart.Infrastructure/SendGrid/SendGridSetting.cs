using Autopart.Application.Models;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Autopart.Infrastructure.SendGrid
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

            // Add attachments to the email message
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

            var response = await client.SendEmailAsync(msg);
            return response.IsSuccessStatusCode;
        }

        //public class AttachmentModel
        //{
        //    public string FilePath { get; set; }
        //    public string FileName { get; set; }
        //    public string MimeType { get; set; }
        //}
    }
}
