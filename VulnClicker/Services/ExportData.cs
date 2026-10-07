using System.Security.Cryptography;
using System.Text;
using VulnClicker.Models;

namespace VulnClicker.Services;

public static class ExportData
{
    public static void Export(
        string filePath,
        GameSaveData saveData)
    {
        // ==============================================
        // 1. 元データ作成
        // ==============================================

        string data =
            $"{saveData.Player}:" +
            $"{saveData.Score}:" +
            $"{saveData.Hit}:" +
            $"{saveData.Miss}:" +
            $"{saveData.Combo}:" +
            $"{saveData.RemainingTime}" ;

        // ==============================================
        // 2. 元データのSHA-256
        // ==============================================

        string hash = CreateHash(data);

        // ==============================================
        // 3. Hashを末尾へ追加
        // ==============================================

        string dataWithHash =
            $"{data}:{hash}";

        // ==============================================
        // 4. 全体をBase64化
        // ==============================================

        byte[] bytes =
            Encoding.UTF8.GetBytes(dataWithHash);

        string base64 =
            Convert.ToBase64String(bytes);

        // ==============================================
        // 5. ファイル保存
        // ==============================================

        File.WriteAllText(
            filePath,
            base64,
            Encoding.UTF8);
    }

    private static string CreateHash(string data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        byte[] hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }
}