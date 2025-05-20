using BeardedCapitalBot.Data;
using BeardedCapitalBot.Extensions;
using BeardedCapitalBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeardedCapitalBot.StateMachine.States;

public class RequestEmailState : ChatStateBase
{
    private readonly UsersDataProvider _usersDataProvider;
    private readonly NotionService _notionService;
    private readonly ITelegramBotClient _botClient;
    
    
    public RequestEmailState(ChatStateMachine stateMachine, ITelegramBotClient botClient, 
        UsersDataProvider usersDataProvider, NotionService notionService) : base(stateMachine)
    {
        _usersDataProvider = usersDataProvider;
        _notionService = notionService;
        _botClient = botClient;
    }

    public override async Task HandleMessage(Message message)
    {
        var chatId = message.Chat.Id;
        var email = message.Text?.Trim();
        
        if (string.IsNullOrEmpty(email) || !email.Contains("@") || !email.Contains("."))
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id, "❗ Пожалуйста, введите корректный email.");
            return;
        }

        if (await _notionService.CheckUserAsync(email))
        {
            await _botClient.SendTextMessageAsync(chatId, "⚠ Этот email уже использовался для получения гайда.");
            await _stateMachine.TransitTo<IdleState>(chatId);
            return;
        }

        
        _usersDataProvider.SetTelegramName(chatId, message.From?.Username);
        _usersDataProvider.SetUserEmail(chatId, email);
        
        await _notionService.AddUserAsync(_usersDataProvider.GetUserData(chatId));

        await _stateMachine.TransitTo<GuideToEmailState>(chatId);
    }

    public override async Task OnEnter(long chatId)
    {
        Console.WriteLine("RequestEmailState");
        
        var response = "Введите свой email, на который вы хотите получить гайд:";
        
        var backButton = InlineKeyboardButton.WithCallbackData("Назад", GlobalData.START);
        
        var keyboard = new InlineKeyboardMarkup(new[] { backButton });

        await _botClient.SafeSendTextMessageAsync(chatId, response.EscapeMarkdownV2(), replyMarkup: keyboard, parseMode: ParseMode.MarkdownV2);
    }
}