using FluentResponse.Interfaces;

namespace ReSR.Application.Ports;
public interface IMailService {
    Task<IResponse> TrySendEmailAsync(string toEmail, string subject, string body);
}