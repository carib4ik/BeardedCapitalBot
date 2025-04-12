using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BeardedCapitalBot.Services;

public class EmailService
{
    private readonly string _smtpHost = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _fromEmail;
    private readonly string _fromPassword;

    public EmailService(string fromEmail, string fromPassword)
    {
        _fromEmail = fromEmail;
        _fromPassword = fromPassword;
    }
    
    public async Task SendTextEmailAsync(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();

        message.From.Add(MailboxAddress.Parse(_fromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        message.Body = new TextPart("plain")
        {
            Text = body
        };

        using var client = new SmtpClient();
        
        Console.WriteLine("Подключение к SMTP-серверу...");
        await client.ConnectAsync(_smtpHost, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        
        Console.WriteLine("Аутентификация...");
        await client.AuthenticateAsync(_fromEmail, _fromPassword);
        
        Console.WriteLine("Отправка письма...");
        
        Console.WriteLine("Письмо успешно отправлено!");
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    // public async Task SendFileToEmailAsync(string toEmail, string subject, string body, string filePath)
    // {
    //     var message = new MimeMessage();
    //     message.From.Add(new MailboxAddress("Boroda Capital", _fromEmail));
    //     message.To.Add(new MailboxAddress("", toEmail));
    //     message.Subject = subject;
    //
    //     var builder = new BodyBuilder
    //     {
    //         TextBody = body
    //     };
    //
    //     if (File.Exists(filePath))
    //     {
    //         builder.Attachments.Add(filePath);
    //     }
    //     else
    //     {
    //         Console.WriteLine("⚠ Файл не найден для отправки");
    //         return;
    //     }
    //
    //     message.Body = builder.ToMessageBody();
    //
    //     using var client = new SmtpClient();
    //
    //     try
    //     {
    //         Console.WriteLine("🔌 Подключение к SMTP-серверу...");
    //         await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
    //
    //         Console.WriteLine("🔑 Аутентификация...");
    //         await client.AuthenticateAsync(_fromEmail, _fromPassword);
    //
    //         Console.WriteLine("📤 Отправка письма...");
    //         await client.SendAsync(message);
    //
    //         Console.WriteLine("✅ Письмо успешно отправлено!");
    //
    //         await client.DisconnectAsync(true);
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"❌ Ошибка при отправке: {ex.Message}");
    //         throw;
    //     }
    // }
}