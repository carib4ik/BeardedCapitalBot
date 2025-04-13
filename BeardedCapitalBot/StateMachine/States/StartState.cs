using BeardedCapitalBot.Data;
using BeardedCapitalBot.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeardedCapitalBot.StateMachine.States;

public class StartState : ChatStateBase
{
    private readonly ITelegramBotClient _botClient;
    
    public StartState(ChatStateMachine stateMachine, ITelegramBotClient botClient) : base(stateMachine)
    {
        _botClient = botClient;
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }
    
    public override async Task OnEnter(long chatId)
    {
        Console.WriteLine("StartState");

        await SendGreeting(chatId);
    }

    private async Task SendGreeting(long chatId)
    {
        var greetings = $"Какая-то инфа. Стартовое сообщение";
        
        var guideButton = InlineKeyboardButton.WithCallbackData("Гайд", GlobalData.GUIDE);
        var infoButton = InlineKeyboardButton.WithCallbackData("Информация", GlobalData.INFO);
        var surfingButton = InlineKeyboardButton.WithCallbackData("Серф Кэмп", GlobalData.SURFING);
        var premiumButton = InlineKeyboardButton.WithUrl("Премиум канал", GlobalData.PREMIUM_CHANEL);
        
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[] { guideButton, infoButton },
            new[] { surfingButton, premiumButton }
        });
        
        await _botClient.SafeSendTextMessageAsync(chatId, greetings.EscapeMarkdownV2(), replyMarkup: keyboard, parseMode: ParseMode.MarkdownV2);
        await _stateMachine.TransitTo<IdleState>(chatId);
    }
}