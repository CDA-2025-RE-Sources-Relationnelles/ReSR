using System.Net;
using System.Net.Mail;
using ReSR.Application.Ports;
using FluentResponse;
using FluentResponse.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ReSR.Infrastructure.Adapters;
internal class MailService(
    string  host,
    ushort  port,
    string  senderEmail,
    string? senderPassword = null
) : IMailService {

    #region PROPERTIES

        private readonly string      host           = host;
        private readonly ushort      port           = port;
        private readonly MailAddress senderEmail    = new(senderEmail);
        private readonly string?     senderPassword = senderPassword;

    #endregion
    #region CONSTRUCTORS

        public MailService(IConfiguration configuration) : this(
            host           : configuration["Smtp:Host"]!,
            port           : ushort.Parse(configuration["Smtp:Port"]!),
            senderEmail    : configuration["Smtp:SenderEmail"]!,
            senderPassword : configuration["Smtp:SenderPassword"]!
        ) {}

    #endregion
    #region METHODS

        public Task<IResponse> TrySendEmailAsync(
            string toEmail,
            string subject,
            string body
        ) => Response.WrapAsync(async () => {
            if (toEmail.IsWhiteSpace()) return;

            using MailMessage myMail = new(this.senderEmail, new (toEmail)) {

                Subject         = subject,
                SubjectEncoding = System.Text.Encoding.UTF8,

                Body         = body,
                BodyEncoding = System.Text.Encoding.UTF8,
            };

            using SmtpClient mySmtpClient = new(this.host, this.port)  {
                UseDefaultCredentials = false,
                Credentials           = new NetworkCredential(this.senderEmail.Address, this.senderPassword)
            };

            await mySmtpClient.SendMailAsync(myMail);
        });

    #endregion

}