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
        ManagerChannelId = ParseChatId(Environment.GetEnvironmentVariable("MANAGER_CHANNEL_ID"));
        SubscribeChannelId = ParseChatId(Environment.GetEnvironmentVariable("SUBSCRIBE_CHANNEL_ID"));
        ErrorsLogChannelId = ParseChatId(Environment.GetEnvironmentVariable("ERRORS_LOG_CHANNEL_ID"));
        ErrorsFilePath = Environment.GetEnvironmentVariable("ERRORS_FILE_PATH") ?? "ErrorsLog.txt";
    }

    private ChatId ParseChatId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("ChatId cannot be empty.");
        }

        return long.TryParse(value, out var chatId) ? new ChatId(chatId) : new ChatId(value);
    }

}