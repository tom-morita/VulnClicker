using System.Text.Json;
using VulnClickerScoreServer.Models;

namespace VulnClickerScoreServer.Services;

public class ScoreRepository
{
    private readonly string _filePath;
    private readonly object _lock = new();

    private readonly bool _useDefaultRanking;
    private readonly string _serverVersion;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };


    // ==================================================
    // コンストラクタ
    // ==================================================

    public ScoreRepository(
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        var dataDirectory =
            Path.Combine(
                environment.ContentRootPath,
                "Data"
            );

        Directory.CreateDirectory(dataDirectory);

        _filePath =
            Path.Combine(
                dataDirectory,
                "scores.json"
            );

        _useDefaultRanking =
            configuration.GetValue<bool?>(
                "Ranking:UseDefaultRanking"
            ) ?? true;

        _serverVersion =
            configuration["ScoreSettings:ServerVersion"]
            ?? "1.0.0";


        // サーバー起動時に必ずランキングを初期状態へ戻す
        ResetToInitialRanking();
    }


    // ==================================================
    // 全スコア取得
    // ==================================================

    public List<ScoreEntry> GetAll()
    {
        lock (_lock)
        {
            return Load();
        }
    }


    // ==================================================
    // スコア追加
    // ==================================================

    public void Add(
        ScoreEntry entry)
    {
        lock (_lock)
        {
            var scores = Load();

            scores.Add(entry);

            Save(scores);
        }
    }


    // ==================================================
    // ランキング取得
    // ==================================================

    public List<ScoreEntry> GetRanking(
        int maxEntries)
    {
        lock (_lock)
        {
            if (maxEntries <= 0)
            {
                maxEntries = 10;
            }

            return SortRanking(Load())
                .Take(maxEntries)
                .ToList();
        }
    }

    // ==================================================
    // コンボランキング取得
    // ==================================================

    public List<ScoreEntry> GetComboRanking(
        int maxEntries)
    {
        lock (_lock)
        {
            // 不正な設定値への最低限の保護
            if (maxEntries <= 0)
            {
                maxEntries = 10;
            }

            return Load()
                .OrderByDescending(
                    x => x.Combo
                )
                .ThenByDescending(
                    x => x.Score
                )
                .ThenBy(
                    x => x.Miss
                )
                .ThenBy(
                    x => x.Timestamp
                )
                .Take(maxEntries)
                .ToList();
        }
    }

    // ==================================================
    // 指定順位のスコア削除
    // ==================================================

    public bool DeleteByRank(
        int rank,
        int maxEntries,
        out ScoreEntry? deletedEntry)
    {
        lock (_lock)
        {
            deletedEntry = null;

            if (rank <= 0)
            {
                return false;
            }

            if (maxEntries <= 0)
            {
                maxEntries = 10;
            }

            var scores = Load();

            var ranking =
                SortRanking(scores)
                    .Take(maxEntries)
                    .ToList();

            if (rank > ranking.Count)
            {
                return false;
            }

            deletedEntry = ranking[rank - 1];

            scores.Remove(deletedEntry);

            Save(scores);

            return true;
        }
    }


    // ==================================================
    // 起動時ランキング初期化
    // ==================================================

    private void ResetToInitialRanking()
    {
        lock (_lock)
        {
            // デフォルトランキングを使用しない場合
            // 完全に空の状態から開始
            if (!_useDefaultRanking)
            {
                Save(new List<ScoreEntry>());
                return;
            }

            // デフォルトランキングへロールバック
            Save(CreateDefaultScores());
        }
    }


    // ==================================================
    // デフォルトスコア
    // ==================================================

    private List<ScoreEntry> CreateDefaultScores()
    {
        return new List<ScoreEntry>
        {
            new()
            {
                Player = "OWASP Kansai",
                Score = 170,
                Hit = 17,
                Miss = 17,
                Combo = 17,
                Timestamp =
                    new DateTimeOffset(
                        2026, 9, 22,
                        15, 30, 0,
                        TimeSpan.FromHours(9)
                    ),
                Version = _serverVersion
            },

            new()
            {
                Player = "sample player",
                Score = 120,
                Hit = 1,
                Miss = 1,
                Combo = 1,
                Timestamp =
                    new DateTimeOffset(
                        2026, 9, 22,
                        15, 33, 0,
                        TimeSpan.FromHours(9)
                    ),
                Version = _serverVersion
            },

            new()
            {
                Player = "hello",
                Score = 110,
                Hit = 110,
                Miss = 10,
                Combo = 10,
                Timestamp =
                    new DateTimeOffset(
                        2026, 9, 24,
                        1, 52, 5,
                        TimeSpan.FromHours(9)
                    ),
                Version = _serverVersion
            },

            new()
            {
                Player = "I_LOVE_dotNET",
                Score = 10,
                Hit = 11,
                Miss = 0,
                Combo = 11,
                Timestamp =
                    new DateTimeOffset(
                        2026, 9, 24,
                        1, 53, 10,
                        TimeSpan.FromHours(9)
                    ),
                Version = _serverVersion
            }
        };
    }


    // ==================================================
    // ランキングソート
    // ==================================================

    private static IOrderedEnumerable<ScoreEntry> SortRanking(
        IEnumerable<ScoreEntry> scores)
    {
        return scores
            .OrderByDescending(
                x => x.Score
            )
            .ThenBy(
                x => x.Miss
            )
            .ThenBy(
                x => x.Timestamp
            );
    }


    // ==================================================
    // JSON読み込み
    // ==================================================

    private List<ScoreEntry> Load()
    {
        try
        {
            var json =
                File.ReadAllText(
                    _filePath
                );

            return JsonSerializer
                .Deserialize<List<ScoreEntry>>(
                    json,
                    _jsonOptions
                )
                ?? new List<ScoreEntry>();
        }
        catch
        {
            return new List<ScoreEntry>();
        }
    }


    // ==================================================
    // JSON保存
    // ==================================================

    private void Save(
        List<ScoreEntry> scores)
    {
        var json =
            JsonSerializer.Serialize(
                scores,
                _jsonOptions
            );

        File.WriteAllText(
            _filePath,
            json
        );
    }
}