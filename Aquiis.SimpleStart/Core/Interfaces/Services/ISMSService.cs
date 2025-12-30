
namespace Aquiis.SimpleStart.Core.Interfaces.Services;
public interface ISMSService
{
    Task SendSMSAsync(string phoneNumber, string message);
    Task<bool> ValidatePhoneNumberAsync(string phoneNumber);
}