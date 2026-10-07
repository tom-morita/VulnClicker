using System.Diagnostics;
using System.Net.Http.Json;
using VulnClicker.Models;

namespace VulnClicker.Services;

public class ScoreClient
{
    private readonly HttpClient _httpClient;

    private string ServerUrl = "https://localhost:8443;http://localhost:8080";

    private readonly StartOptions startOptions;
    public ScoreClient(StartOptions startOptions)
    {
        this.startOptions = startOptions;
        if (startOptions.Profile == RuntimeProfile.Diagnostic)
        {
            ServerUrl = ServerUrl.Split(';')[1];
        }
        else
        {
            ServerUrl = ServerUrl.Split(';')[0];
        }
    

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(ServerUrl)
        };

        _httpClient.Timeout = TimeSpan.FromSeconds(5);
    }

    /// <summary>
    /// スコアをサーバーへ登録します。
    /// </summary>
    public async Task<bool> RegisterScoreAsync(
        Models.ScoreEntry score)
    {
        try
        {
            using HttpResponseMessage response =
                await _httpClient.PostAsJsonAsync(
                    "/api/scores",
                    score);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }

    /// <summary>
    /// サーバーから上位10件を取得します。
    /// </summary>
    public async Task<List<Models.RankingEntry>> GetTop10Async()
    {
        try
        {
            var result =
                await _httpClient.GetFromJsonAsync<List<Models.RankingEntry>>(
                    "/api/scores/top10");

            return result ?? new List<Models.RankingEntry>();
        }
        catch (HttpRequestException)
        {
            return new List<Models.RankingEntry>();
        }
        catch (TaskCanceledException)
        {
            return new List<Models. RankingEntry>();
        }
    }
}