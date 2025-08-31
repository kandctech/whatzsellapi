

namespace XYZ.WShop.Application.Interfaces.Services
{
    public interface IPushNotificationService
    {
        Task SendPushNotification(List<string> to, string title, string body, object data = null);

    }
}
