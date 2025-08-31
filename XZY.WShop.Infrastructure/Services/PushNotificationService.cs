using Expo.Server.Client;
using Expo.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Interfaces.Services;

namespace XZY.WShop.Infrastructure.Services
{
    public class PushNotificationService: IPushNotificationService
    {
        public async Task SendPushNotification(List<string> to, string title, string body, object data = null!)
        {
            var expoSDKClient = new PushApiClient();
            var pushTicketReq = new PushTicketRequest()
            {
                PushTo = to,
                PushTitle = title,
                PushBody = body,
                PushData = data

            };
            var result = await expoSDKClient.PushSendAsync(pushTicketReq);

            if (result?.PushTicketErrors?.Count() > 0)
            {
                foreach (var error in result.PushTicketErrors)
                {
                    Console.WriteLine($"Error: {error.ErrorCode} - {error.ErrorMessage}");
                    SentrySdk.CaptureMessage(error.ErrorMessage);
                }
            }

        }
    }
}
