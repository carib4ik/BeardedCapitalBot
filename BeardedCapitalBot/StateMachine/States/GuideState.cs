using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

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
        // await _botClient.SendDocumentAsync(chatId, document: new InputFileId("BQACAgIAAxkBAANxZ9qz4WuSakrNw38rbAXpVzPMsb4AAspnAAK2L9FK-bzsZjeJeho2BA"), caption:"Держи гайд");
        await SendStoredFileAsync(chatId);
        await _stateMachine.TransitTo<IdleState>(chatId);
    }
    
    private string? LoadFileId()
    {
        if (!File.Exists("file_id.json"))
        {
            return null; // Файл не найден
        }

        var json = File.ReadAllText("file_id.json");
        var fileData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        return fileData != null && fileData.ContainsKey("FileId") ? fileData["FileId"] : null;
    }

    public async Task SendStoredFileAsync(ChatId chatId)
    {
        var fileId = LoadFileId();

        if (string.IsNullOrEmpty(fileId))
        {
            await _botClient.SendTextMessageAsync(chatId, "⚠ Файл не найден, загрузите новый.");
            return;
        }

        await _botClient.SendDocumentAsync(chatId, new InputFileId(fileId), caption: "📄 Держи гайд!");
    }

}