using BeardedCapitalBot.Data;
using BeardedCapitalBot.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeardedCapitalBot.StateMachine.States;

public class GuideState : ChatStateBase
{
    private readonly ITelegramBotClient _botClient;
    
    public GuideState(ChatStateMachine stateMachine, ITelegramBotClient botClient) : base(stateMachine)
    {
        _botClient = botClient;
        Console.WriteLine("GuideState");
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }

    public override async Task OnEnter(long chatId)
    {
        var guide = $"тут текст гайда";
        
        var backButton = InlineKeyboardButton.WithCallbackData("Назад", GlobalData.BACK);
        // var infoButton = InlineKeyboardButton.WithCallbackData("Информация", GlobalData.INFO);
        // var surfingButton = InlineKeyboardButton.WithCallbackData("Серф Кэмп", GlobalData.SURFING);
        // var premiumButton = InlineKeyboardButton.WithCallbackData("Премиум канал", GlobalData.PREMIUM_CHANEL);
        
        var keyboard = new InlineKeyboardMarkup( new[] { backButton });
        
        await _botClient.SafeSendTextMessageAsync(chatId, guide.EscapeMarkdownV2(), replyMarkup: keyboard, parseMode: ParseMode.MarkdownV2);
        await _stateMachine.TransitTo<IdleState>(chatId);
    }
}