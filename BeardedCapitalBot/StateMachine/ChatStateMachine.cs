using System.Collections.Concurrent;
using BeardedCapitalBot.Services;
using BeardedCapitalBot.StateMachine.States;
using Telegram.Bot;

namespace BeardedCapitalBot.StateMachine;

public class ChatStateMachine
{
    private readonly ConcurrentDictionary<long, ChatStateBase> _chatStates = new();
    private readonly Dictionary<Type, Func<ChatStateBase>> _states = new();
    
    public ChatStateMachine(ITelegramBotClient botClient, AppSettings.AppSettings appSettings, UsersDataProvider usersDataProvider)
    {
        _states[typeof(IdleState)] = () => new IdleState(this);
        _states[typeof(StartState)] = () => new StartState(this, botClient);
        _states[typeof(WaitingForNameState)] = () => new WaitingForNameState(this, botClient, usersDataProvider);
        _states[typeof(WaitingForPhoneState)] = () => new WaitingForPhoneState(this, botClient, usersDataProvider);
        _states[typeof(UserDataSubmissionState)] = () => new UserDataSubmissionState(this, botClient, usersDataProvider, appSettings.ManagerChannelId);
        _states[typeof(DoneState)] = () => new DoneState(this, botClient);
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