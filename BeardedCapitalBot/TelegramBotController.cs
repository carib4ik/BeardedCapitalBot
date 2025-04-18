using BeardedCapitalBot.Data;
using BeardedCapitalBot.Extensions;
using BeardedCapitalBot.Services;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace BeardedCapitalBot;

public class TelegramBotController
{
    private readonly ITelegramBotClient _botClient;
    private readonly SubscriptionService _subscriptionService;
    private readonly ChatStateController _chatStateController;

    public TelegramBotController(ITelegramBotClient telegramBotClient, SubscriptionService subscriptionService, ChatStateController chatStateController)
    {
        _botClient = telegramBotClient;
        _subscriptionService = subscriptionService;
        _chatStateController = chatStateController;
    }

    public void StartBot()
    {
        using var cts = new CancellationTokenSource();
        
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery]
        };

        CreateCommandsKeyboard().WaitAsync(cts.Token);
        
        _botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken: cts.Token);

        Console.WriteLine("Bot started");
    }

    private async Task CreateCommandsKeyboard()
    {
        await _botClient.DeleteMyCommandsAsync();

        var commands = new[] { new BotCommand { Command = GlobalData.START, Description = "Запустить бота" } };

        await _botClient.SetMyCommandsAsync(commands);
    }
    
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, 
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Update received: {update.Type}");
        
        if (update.Message == null && update.CallbackQuery == null)
        {
            return;
        }

        var message = update.Message;
        var callbackQuery = update.CallbackQuery;

        if (message != null && message.Type != MessageType.Text && message.Type != MessageType.Document)
        {
            return;
        }
        
        var userId = message != null ? message.From.Id : callbackQuery.From.Id;
        var messageId = message != null ? message.MessageId : callbackQuery.Message.MessageId;
        var messageText = message != null ? message.Text : callbackQuery?.Data;
        var chatId = message?.Chat.Id;
        
        string response;
        
        if (messageText == GlobalData.CHECK_SUBSCRIPTION)
        {
            if (!await _subscriptionService.IsSubscribed(userId))
            {
                response = "Для продолжения работы бота, Вам необходимо быть подписчиком канала";
                
                await DeleteMessageAsync(userId, messageId, cancellationToken);
                await SendSubscriptionMessage(userId, response);
                return;
            }
            
            await DeleteMessageAsync(userId, messageId, cancellationToken);
        }
        
        if (await _subscriptionService.IsSubscribed(userId))
        {
            // response = "Вы подписаны";
            // await _botClient.SendTextMessageAsync(userId, response, cancellationToken: cancellationToken);
            await _chatStateController.HandleUpdateAsync(update);
        }
        else
        {
            response = "Безлимитное использование бота доступно подписчикам канала";
            await SendSubscriptionMessage(userId, response);
        }
        
        // Проверяем, загрузил ли администратор документ
        if (update.Message?.Document != null)
        {
            await TryToSaveFile(chatId, message);
        }
    }

    private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var requestException = exception switch
        {
            ApiRequestException apiRequestException => apiRequestException,
            _ => exception
        };

        var errorText = "Произошла критическая ошибка. Требуется *ПЕРЕЗАПУСК* бота";
        await ErrorNotificationService.Instance.SendTextMessageError(errorText);
        await ErrorNotificationService.Instance.SendErrorNotification(requestException);
    }

    private async Task SendSubscriptionMessage(long chatId, string response)
    {
        try
        {
            var channelInfo = await _botClient.GetChatAsync(_subscriptionService.SubscribeChatId);
            
            var channelName = channelInfo.Title.EscapeMarkdownV2();
            var channelUsername = channelInfo.Username;
            var channelLink = $"[{channelName}](https://t.me/{channelUsername})";
            
            var subscriptionMessage = $"{response}\n\nПодпишитесь на канал: {channelLink}";
            
            var keyboardButton = new InlineKeyboardButton("Проверить подписку")
            {
                CallbackData = GlobalData.CHECK_SUBSCRIPTION
            };
            
            var inlineKeyboardMarkup = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    keyboardButton
                }
            });
            
            await _botClient.SafeSendTextMessageAsync(chatId, subscriptionMessage,
                replyMarkup: inlineKeyboardMarkup,
                parseMode: ParseMode.MarkdownV2);
        }
        
        catch (ApiRequestException exception)
        {
            await ErrorNotificationService.Instance.SendErrorNotification(exception);
        }
    }
    
    private async Task DeleteMessageAsync(long chatId, int messageId, CancellationToken cancellationToken)
    {
        try
        {
            await _botClient.DeleteMessageAsync(chatId, messageId, cancellationToken: cancellationToken);
        }
        catch (ApiRequestException exception)
        {
            await ErrorNotificationService.Instance.SendErrorNotification(exception);
        }
    }
    
    private async Task TryToSaveFile(ChatId chatId, Message message)
    {
        if (message.From.Id != GlobalData.ADMIN_ID) // Если не админ, запрещаем загрузку
        {
            await _botClient.SendTextMessageAsync(chatId, "❌ У вас нет прав загружать файлы.");
            return;
        }

        // Сохраняем FileId
        var fileId = message.Document.FileId;
        var json = $"{{ \"FileId\": \"{fileId}\" }}";
        await File.WriteAllTextAsync("file_id.json", json);

        await _botClient.SendTextMessageAsync(chatId, $"✅ FileId сохранён: `{fileId}`", parseMode: ParseMode.Markdown);
        return; // Завершаем обработку, если это был документ
    }
}