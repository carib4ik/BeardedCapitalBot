using Telegram.Bot.Types;

namespace BeardedCapitalBot.AppSettings;

public class AppSettings
{
    public ChatId ManagerChannelId { get; init; }
    public ChatId SubscribeChannelId { get; init; }
    public ChatId ErrorsLogChannelId { get; init; }
    public string ErrorsFilePath { get; init; }
}