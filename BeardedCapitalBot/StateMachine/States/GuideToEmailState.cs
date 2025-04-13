using BeardedCapitalBot.Data;
using BeardedCapitalBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace BeardedCapitalBot.StateMachine.States;

public class GuideToEmailState : ChatStateBase
{
    private readonly ITelegramBotClient _botClient;
    private readonly UsersDataProvider _usersDataProvider;
    private readonly EmailService _emailService;

    public GuideToEmailState(ChatStateMachine stateMachine, ITelegramBotClient botClient,
        UsersDataProvider usersDataProvider, EmailService emailService) : base(stateMachine)
    {
        _botClient = botClient;
        _usersDataProvider = usersDataProvider;
        _emailService = emailService;
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }

    public override async Task OnEnter(long chatId)
    {
        Console.WriteLine("GuideToEmailState");

        await SendStoredFileAsync(chatId);
        await _stateMachine.TransitTo<IdleState>(chatId);
    }

    private async Task SendStoredFileAsync(long chatId)
    {
        var userEmail = _usersDataProvider.GetUserData(chatId).Email;

        if (userEmail == null)
        {
            Console.WriteLine("user email is not found");
            return;
        }
        
        try
        {
            await _emailService.SendTextEmailAsync(
                toEmail: userEmail,
                subject: "Ваш гайд по крипте",
                body: GlobalData.EMAIL_BODY
            );
            
            await _botClient.SendTextMessageAsync(chatId, $"Гайд успешно отправлен на ваш email {userEmail}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка при отправке файла на email: {ex.Message}");
        }
        
    }

}