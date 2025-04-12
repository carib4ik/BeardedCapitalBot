namespace BeardedCapitalBot.AppSettings;

public class Secrets
{
    public string TelegramKey { get; init; }
    public string FromEmail { get; init; }
    public string FromEmailPassword { get; init; }

    // public Secrets()
    // {
    //     TelegramKey = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") 
    //                   ?? throw new Exception("TELEGRAM_BOT_TOKEN not set");
    // }
}