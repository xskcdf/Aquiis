
using Aquiis.SimpleStart.Core.Interfaces.Services;

namespace Aquiis.SimpleStart.Infrastructure.Services;

public class SMSService : ISMSService
{
    private readonly ILogger<SMSService> _logger;

    public SMSService(ILogger<SMSService> logger)
    {
        _logger = logger;
    }

    public async Task SendSMSAsync(string phoneNumber, string message)
    {
        // TODO: Implement with Twilio in Task 2.5
        _logger.LogInformation($"[SMS] To: {phoneNumber}, Message: {message}");
        await Task.CompletedTask;
    }

    public async Task<bool> ValidatePhoneNumberAsync(string phoneNumber)
    {
        // Basic validation
        var digits = new string(phoneNumber.Where(char.IsDigit).ToArray());
        return await Task.FromResult(digits.Length >= 10);
    }
}