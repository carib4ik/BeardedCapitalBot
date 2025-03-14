using Telegram.Bot.Types;

namespace BeardedCapitalBot.AppSettings;

public class AppSettings
{
    public ChatId ManagerChannelId { get; init; }
    public ChatId SubscribeChannelId { get; init; }
    public ChatId ErrorsLogChannelId { get; init; }
    public string ErrorsFilePath { get; init; }
    
    
    public AppSettings()
    {
        ManagerChannelId = Environment.GetEnvironmentVariable("MANAGER_CHANNEL_ID") ?? "@default";
        SubscribeChannelId = Environment.GetEnvironmentVariable("SUBSCRIBE_CHANNEL_ID") ?? "@default";
        ErrorsLogChannelId = Environment.GetEnvironmentVariable("ERRORS_LOG_CHANNEL_ID") ?? "@default";
        ErrorsFilePath = Environment.GetEnvironmentVariable("ERRORS_FILE_PATH") ?? "ErrorsLog.txt";
    }
}