using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

class Program
{    static async Task Main(string[] args)
    {
        Console.WriteLine("Enter the word and get the deffinition : ");
        string word = Console.ReadLine();
        string apiUrl = $"https://api.dictionaryapi.dev/api/v2/entries/en/{word}";

        using (HttpClient client = new HttpClient())
        {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {                    string responseBody = await response.Content.ReadAsStringAsync();
                    var dictionaryData = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    var meanings = dictionaryData[0].GetProperty("meanings");
                    var firstMeaning = meanings[0].GetProperty("definitions")[0].GetProperty("definition").GetString();

                    Console.WriteLine($"Word: {word}");
                    Console.WriteLine($"Definition: {firstMeaning}");
                }
                else{Console.WriteLine($"Error: {response.StatusCode}");}          
        }
    }
}