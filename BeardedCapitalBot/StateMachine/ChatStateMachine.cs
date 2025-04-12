using System.Collections.Concurrent;
using BeardedCapitalBot.Services;
using BeardedCapitalBot.StateMachine.States;
using Telegram.Bot;

namespace BeardedCapitalBot.StateMachine;

public class ChatStateMachine
{
    private readonly ConcurrentDictionary<long, ChatStateBase> _chatStates = new();
    private readonly Dictionary<Type, Func<ChatStateBase>> _states = new();
    
    public ChatStateMachine(ITelegramBotClient botClient, AppSettings.AppSettings appSettings,
        UsersDataProvider usersDataProvider, EmailService emailService)
    {
        _states[typeof(IdleState)] = () => new IdleState(this);
        _states[typeof(StartState)] = () => new StartState(this, botClient);
        _states[typeof(RequestEmailState)] = () => new RequestEmailState(this, botClient, usersDataProvider);
        _states[typeof(DoneState)] = () => new DoneState(this, botClient);
        _states[typeof(GuideSendToEmailState)] = () => new GuideSendToEmailState(this, botClient, usersDataProvider, emailService);
        _states[typeof(InfoState)] = () => new InfoState(this, botClient);
        _states[typeof(SurfCampState)] = () => new SurfCampState(this, botClient);
    }
    
    public ChatStateBase GetState(long chatId)
    {
        return !_chatStates.TryGetValue(chatId, out var state) ? _states[typeof(IdleState)].Invoke() : state;
    }
    
    public async Task TransitTo<T>(long chatId) where T : ChatStateBase
    {
        if (_chatStates.TryGetValue(chatId, out var currentState))
        {
            await currentState.OnExit(chatId);
        }

        var newState = _states[typeof(T)]();
        _chatStates[chatId] = newState;
        await newState.OnEnter(chatId);
    }
}