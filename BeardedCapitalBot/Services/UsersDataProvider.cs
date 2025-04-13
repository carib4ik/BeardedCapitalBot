using System.Collections.Concurrent;
using BeardedCapitalBot.Data;

namespace BeardedCapitalBot.Services;

public class UsersDataProvider
{
    private readonly ConcurrentDictionary<long, UserData> _usersData = new();
    
    public void SetTelegramName(long chatId, string? telegramName)
    {
        var userState = _usersData.GetOrAdd(chatId, new UserData());
        userState.TelegramName = telegramName;
    }
    
    public void SetUserEmail(long chatId, string? email)
    {
        
        var userState = _usersData.GetOrAdd(chatId, new UserData());
        userState.Email = email;
    }
    
    public UserData GetUserData(long chatId)
    {
        return _usersData[chatId];
    }

    public void ClearUserData(long chatId)
    {
        _usersData.TryRemove(chatId, out _);
    }
}