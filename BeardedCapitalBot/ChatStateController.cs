using BeardedCapitalBot.Data;
using BeardedCapitalBot.StateMachine;
using BeardedCapitalBot.StateMachine.States;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BeardedCapitalBot;

public class ChatStateController
{
    private readonly ChatStateMachine _stateMachine;

    public ChatStateController(ChatStateMachine chatStateMachine)
    {
        _stateMachine = chatStateMachine;
    }

    public async Task HandleUpdateAsync(Update update)
    {
        if (update.Message == null && update.CallbackQuery == null)
        {
            return;
        }

        string? data;
        Message message;
        
        switch (update.Type)
        {
            case UpdateType.Message:
                data = update.Message.Text;
                message = update.Message;
                break;
            
            case UpdateType.CallbackQuery:
                data = update.CallbackQuery.Data;
                message = update.CallbackQuery.Message;
                break;
            
            default:
                return;
        }
        
        var chatId = message.Chat.Id;
        
        switch (data)
        {
            case GlobalData.START:
                await _stateMachine.TransitTo<StartState>(chatId);
                break;
            
            case GlobalData.CHECK_SUBSCRIPTION:
                await _stateMachine.TransitTo<StartState>(chatId);
                break;
            
            case GlobalData.DONE:
                await _stateMachine.TransitTo<DoneState>(chatId);
                break;
            
            default:
                var state = _stateMachine.GetState(chatId);
                await state.HandleMessage(message);
                break;
        }
    }
}