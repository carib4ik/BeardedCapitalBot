using Telegram.Bot.Types;

namespace BeardedCapitalBot.StateMachine.States;

public class IdleState : ChatStateBase
{
    public IdleState(ChatStateMachine stateMachine) : base(stateMachine)
    {
        Console.WriteLine("IdleState");
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }
}