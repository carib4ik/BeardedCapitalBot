using BeardedCapitalBot.Services;
using Telegram.Bot.Types;

namespace BeardedCapitalBot.StateMachine.States;

public class IdleState : ChatStateBase
{
    private readonly UsersDataProvider _usersDataProvider;
    
    public IdleState(ChatStateMachine stateMachine, UsersDataProvider usersDataProvider) : base(stateMachine)
    {
        _usersDataProvider = usersDataProvider;
    }

    public override async Task OnEnter(long chatId)
    {
        Console.WriteLine("IdleState");
        _usersDataProvider.ClearUserData(chatId);
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }
}