using Newtonsoft.Json;
using BeardedCapitalBot.AppSettings;
using BeardedCapitalBot.Services;
using BeardedCapitalBot.StateMachine;
using Telegram.Bot;

namespace BeardedCapitalBot;

public static class Program
{
    public static async Task Main(string[] args)
    {
        // var secretsJson = await File.ReadAllTextAsync("AppSettings/secrets.json");
        // var secrets = JsonConvert.DeserializeObject<Secrets>(secretsJson);
        //
        // var settingsJson = await File.ReadAllTextAsync("AppSettings/app_settings.json");
        // var settings = JsonConvert.DeserializeObject<BeardedCapitalBot.AppSettings.AppSettings>(settingsJson);

        var secrets = new Secrets();
        var settings = new AppSettings.AppSettings();
        
        var botClient = new TelegramBotClient(secrets.TelegramKey);
        
        ErrorNotificationService.Initialize(botClient, settings.ErrorsLogChannelId, settings.ErrorsFilePath);
        
        var subscriptionService = new SubscriptionService(botClient, settings.SubscribeChannelId);
        var usersDataProvider = new UsersDataProvider();
        
        var chatStateMachine = new ChatStateMachine(botClient, settings, usersDataProvider);
        var chatStateController = new ChatStateController(chatStateMachine);
        
        var telegramBot = new TelegramBotController(botClient, subscriptionService, chatStateController);
        
        telegramBot.StartBot();
        await Task.Delay(Timeout.Infinite);

    }
}