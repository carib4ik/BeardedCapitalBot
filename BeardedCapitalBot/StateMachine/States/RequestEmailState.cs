using BeardedCapitalBot.Extensions;
using BeardedCapitalBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BeardedCapitalBot.StateMachine.States;

public class RequestEmailState : ChatStateBase
{
    private readonly UsersDataProvider _usersDataProvider;
    private readonly ITelegramBotClient _botClient;
    
    
    public RequestEmailState(ChatStateMachine stateMachine, ITelegramBotClient botClient, 
        UsersDataProvider usersDataProvider) : base(stateMachine)
    {
        _usersDataProvider = usersDataProvider;
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

        // if (IsEmailUsed(chatId, email))
        // {
        //     await _botClient.SendTextMessageAsync(chatId, "⚠ Этот email уже использовался для получения гайда.");
        //     await _stateMachine.TransitTo<IdleState>(chatId);
        //     return;
        // }

        
        _usersDataProvider.SetTelegramName(chatId, message.From?.Username);
        _usersDataProvider.SetUserEmail(chatId, email);

        await _stateMachine.TransitTo<GuideToEmailState>(chatId);
    }

    public override async Task OnEnter(long chatId)
    {
        await base.OnEnter(chatId);

        var response = "Введите свой email, на который вы хотите получить гайд:";
        await _botClient.SafeSendTextMessageAsync(chatId, response);
    }
    
    private bool IsEmailUsed(long chatId, string email)
    {
        return _usersDataProvider.GetUserData(chatId).Email == email;
    }
}