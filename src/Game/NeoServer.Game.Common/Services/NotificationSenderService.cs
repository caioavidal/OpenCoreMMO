using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Services;

public static class NotificationSenderService
{
    public static event SendNotification OnNotificationSent;

    public static void Send(IPlayer to, string message,
        NotificationType notificationType = NotificationType.Description)
    {
        OnNotificationSent?.Invoke(to, message, notificationType);
    }
}

public delegate void SendNotification(IPlayer player, string message, NotificationType notificationType);