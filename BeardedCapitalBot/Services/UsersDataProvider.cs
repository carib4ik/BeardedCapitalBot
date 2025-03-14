using System.Collections.Concurrent;
using BeardedCapitalBot.Data;

namespace BeardedCapitalBot.Services;

public class UsersDataProvider
{
    private readonly ConcurrentDictionary<long, UserData> _userDatas = new();

    public void SetUserName(long chatId, string name)
    {
        var userState = _userDatas.GetOrAdd(chatId, new UserData());
        userState.Name = name;
    }
    
    public void SetTelegramName(long chatId, string? telegramName)
    {
        var userState = _userDatas.GetOrAdd(chatId, new UserData());
        userState.TelegramName = telegramName;
    }
    
    public void SetUserPhone(long chatId, string? phone)
    {
        var userState = _userDatas.GetOrAdd(chatId, new UserData());
        userState.Phone = phone;
    }
    
    public UserData GetUserData(long chatId)
    {
        return _userDatas[chatId];
    }

    public void ClearUserData(long chatId)
    {
        _userDatas.TryRemove(chatId, out _);
    }
}