using BeardedCapitalBot.Data;
using BeardedCapitalBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace BeardedCapitalBot.StateMachine.States;

public class GuideSendToEmailState : ChatStateBase
{
    private readonly ITelegramBotClient _botClient;
    private readonly UsersDataProvider _usersDataProvider;
    private readonly EmailService _emailService;

    public GuideSendToEmailState(ChatStateMachine stateMachine, ITelegramBotClient botClient,
        UsersDataProvider usersDataProvider, EmailService emailService) : base(stateMachine)
    {
        _botClient = botClient;
        _usersDataProvider = usersDataProvider;
        _emailService = emailService;
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

    private async Task SendStoredFileAsync(long chatId)
    {
        if (!File.Exists(GlobalData.GUIDE_FILE_PATH))
        {
            Console.WriteLine("⚠ Файл не найден");
            return;
        }
        
        var userEmail = _usersDataProvider.GetUserData(chatId).Email;

        if (userEmail == null)
        {
            Console.WriteLine("user email is not found");
            return;
        }
        
        try
        {
            await _emailService.SendFileToEmailAsync(
                toEmail: userEmail,
                subject: "Ваш гайд",
                body: "Привет! Во вложении находится обещанный гайд",
                filePath: GlobalData.GUIDE_FILE_PATH
            );
            
            Console.WriteLine("✅ Файл успешно отправлен на email");
            await _botClient.SendTextMessageAsync(chatId, $"Гайд успешно отправлен на ваш email {userEmail}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при отправке файла на email: {ex.Message}");
        }
        
    }

}