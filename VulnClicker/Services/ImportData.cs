using System.Security.Cryptography;
using System.Text;
using VulnClicker.Models;

namespace VulnClicker.Services;

public  static class ImportData
{
    public static GameSaveData Import(string filePath)
    {
        // ==============================================
        // 1. ファイル読み込み
        // ==============================================

        string base64 =
            File.ReadAllText(
                filePath,
                Encoding.UTF8
            ).Trim();

        // ==============================================
        // 2. Base64デコード
        // ==============================================

        byte[] decodedBytes = Convert.FromBase64String(base64);
        string decoded = Encoding.UTF8.GetString(decodedBytes);

        // ==============================================
        // 3. ":" で分割
        //
        // 1:Player
        // 2:Score
        // 3:Hit
        // 4:Miss
        // 5:Combo
        // 6:Hash
        // RemainingTime
        // ==============================================

        string[] parts = decoded.Split(':');

        // ==============================================
        // 4. 入力値の簡易的な解析
        // ==============================================

        if (string.IsNullOrWhiteSpace(parts[0].Trim()) ||

            !int.TryParse(
                parts[1],
                out int score) ||

            !int.TryParse(
                parts[2],
                out int hit) ||

            !int.TryParse(
                parts[3],
                out int miss) ||

            !int.TryParse(
                parts[4],
                out int combo) ||

            !int.TryParse(
                parts[5],
                out int remainingTime)
                )
        {
            throw new InvalidDataException(
                "セーブデータに不正な値が含まれています。"
            );
        }

        string player = parts[0].Trim();
        string storedHash = parts[6];

        // ==============================================
        // 5. Hash対象を再構築
        // ==============================================

        string data =
            $"{player}:" +
            $"{score}:" +
            $"{hit}:" +
            $"{miss}:" +
            $"{combo}:" +
            $"{remainingTime}";

        string expectedHash = CreateHash(data);

        // ==============================================
        // 6. Hash照合
        // ==============================================
        

        if (!IsMatchHash(expectedHash, storedHash))
        {
            throw new InvalidDataException(
                "セーブデータの整合性を確認できませんでした。"
            );
        }

        // ==============================================
        // 7. GameSaveDataへ変換
        // ==============================================

        return new GameSaveData
        {
            Player = player,
            Score = score,
            Hit = hit,
            Miss = miss,
            Combo = combo,
            RemainingTime = remainingTime
        };
    }


    private static string CreateHash(string data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        byte[] hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }
    
    private static bool IsMatchHash(string data1, string data2)
    {

        // nullは拒否
        if (data1 == null || data2 == null)
        {
            return false;
        }

        // 空文字は拒否
        if (data1.Length == 0 || data2.Length == 0)
        {
            return false;
        }

        // 一致しない場合は拒否
        for (int i=0; i < Math.Min(data1.Length, data2.Length); i++)
        {
            if (data1[i] != data2[i])
            {
                return false;
            }
        }
        return true;
    }
}