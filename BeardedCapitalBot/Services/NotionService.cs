using System.Net.Http.Headers;
using BeardedCapitalBot.Data;
using Newtonsoft.Json;

namespace BeardedCapitalBot.Services;

public class NotionService
{
    private readonly string _notionToken;
    private readonly string _databaseId;
    private readonly HttpClient _httpClient;

    public NotionService(string notionToken, string databaseId)
    {
        _notionToken = notionToken;
        _databaseId = databaseId;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _notionToken);
        _httpClient.DefaultRequestHeaders.Add("Notion-Version", "2022-06-28"); // текущая версия API
    }
    
    public async Task AddUserAsync(UserData userData)
    {
        var requestBody = new
        {
            parent = new { database_id = _databaseId },
            properties = new
            {
                Name = new
                {
                    title = new[]
                    {
                        new
                        {
                            text = new
                            {
                                content = userData.TelegramName
                            }
                        }
                    }
                },
                Email = new
                {
                    rich_text = new[]
                    {
                        new
                        {
                            text = new
                            {
                                content = userData.Email
                            }
                        }
                    }
                }
            }

        };

        var json = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://api.notion.com/v1/pages", content);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ Ошибка при отправке в Notion: {error}");
        }
        else
        {
            Console.WriteLine("✅ Пользователь успешно добавлен в Notion DB");
        }
    }

    public async Task<bool> CheckUserAsync(string email)
    {
        var requestBody = new
        {
            filter = new
            {
                property = "Email",
                rich_text = new
                {
                    equals = email
                }
            }
        };

        var json = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var url = $"https://api.notion.com/v1/databases/{_databaseId}/query";

        var response = await _httpClient.PostAsync(url, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"❌ Ошибка при проверке в Notion: {responseBody}");
            return false;
        }

        dynamic result = JsonConvert.DeserializeObject(responseBody);
        int resultsCount = result.results.Count;

        return resultsCount > 0;
    }

}