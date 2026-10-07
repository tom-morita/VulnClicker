using VulnClickerScoreServer.Models;
using VulnClickerScoreServer.Services;

var builder = WebApplication.CreateBuilder(args);


// ==================================================
// 設定読み込み
// ==================================================

// Server
string applicationUrl =
    builder.Configuration["Server:Urls"]
    ?? "https://localhost:8443;http://localhost:8080";


// Security
bool enableHttpsRedirection =
    builder.Configuration.GetValue<bool?>(
        "Security:EnableHttpsRedirection"
    ) ?? true;

bool enableScoreValidation =
    builder.Configuration.GetValue<bool?>(
        "Security:EnableScoreValidation"
    ) ?? true;

bool enableVersionValidation =
    builder.Configuration.GetValue<bool?>(
        "Security:EnableVersionValidation"
    ) ?? true;


// ScoreSettings
string serverVersion =
    builder.Configuration["ScoreSettings:ServerVersion"]
    ?? "1.0.0";

int maxPlayerNameLength =
    builder.Configuration.GetValue<int?>(
        "ScoreSettings:MaxPlayerNameLength"
    ) ?? 32;

int maxVersionLength =
    builder.Configuration.GetValue<int?>(
        "ScoreSettings:MaxVersionLength"
    ) ?? 16;


// Ranking
bool useDefaultRanking =
    builder.Configuration.GetValue<bool?>(
        "Ranking:UseDefaultRanking"
    ) ?? true;

int rankingMaxEntries =
    builder.Configuration.GetValue<int?>(
        "Ranking:MaxEntries"
    ) ?? 10;


// ==================================================
// Webサーバー設定
// ==================================================

builder.WebHost.UseUrls(applicationUrl);


// ==================================================
// DI
// ==================================================

builder.Services.AddSingleton<ScoreRepository>();


var app = builder.Build();


// ==================================================
// HTTPSリダイレクト
// ==================================================

if (enableHttpsRedirection)
{
    app.UseHttpsRedirection();
}


// ==================================================
// サーバー状態確認
// GET /
// ==================================================

app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        service = "VulnClickerScoreServer",
        status = "running",

        settings = new
        {
            httpsRedirection = enableHttpsRedirection,
            scoreValidation = enableScoreValidation,
            versionValidation = enableVersionValidation,
            serverVersion,
            defaultRanking = useDefaultRanking
        }
    });
});


// ==================================================
// スコア登録
// POST /api/scores
// ==================================================

app.MapPost(
    "/api/scores",
    (ScoreEntry entry, ScoreRepository repository) =>
{
    // ==============================================
    // スコアデータ検証
    // ==============================================

    if (enableScoreValidation)
    {
        // Player
        if (string.IsNullOrWhiteSpace(entry.Player))
        {
            return Results.BadRequest(new
            {
                error = "Player is required."
            });
        }


        // Player長
        if (entry.Player.Length > maxPlayerNameLength)
        {
            return Results.BadRequest(new
            {
                error =
                    $"Player must be {maxPlayerNameLength} characters or less."
            });
        }


        // Score
        if (entry.Score < 0)
        {
            return Results.BadRequest(new
            {
                error = "Score must be 0 or greater."
            });
        }


        // Hit
        if (entry.Hit < 0)
        {
            return Results.BadRequest(new
            {
                error = "Hit must be 0 or greater."
            });
        }


        // Miss
        if (entry.Miss < 0)
        {
            return Results.BadRequest(new
            {
                error = "Miss must be 0 or greater."
            });
        }


        // Combo
        if (entry.Combo < 0)
        {
            return Results.BadRequest(new
            {
                error = "Combo must be 0 or greater."
            });
        }


        // Version
        if (string.IsNullOrWhiteSpace(entry.Version))
        {
            return Results.BadRequest(new
            {
                error = "Version is required."
            });
        }


        // Version最大長
        if (entry.Version.Length > maxVersionLength)
        {
            return Results.BadRequest(new
            {
                error =
                    $"Version must be {maxVersionLength} characters or less."
            });
        }
    }


    // ==============================================
    // Version一致確認
    // ==============================================

    if (enableVersionValidation &&
        !string.Equals(
            entry.Version,
            serverVersion,
            StringComparison.Ordinal
        ))
    {
        return Results.BadRequest(new
        {
            error = "Version mismatch.",
            serverVersion,
            clientVersion = entry.Version
        });
    }


    // ==============================================
    // Timestamp
    // ==============================================

    if (entry.Timestamp == default)
    {
        entry.Timestamp = DateTimeOffset.Now;
    }


    // ==============================================
    // 保存
    // ==============================================

    repository.Add(entry);


    return Results.Ok(new
    {
        success = true,
        validation = enableScoreValidation,
        versionValidation = enableVersionValidation,
        message = "Score registered."
    });
});


// ==================================================
// ランキング取得
//
// GET /api/scores/top10
//
// sort:
//   未指定 → score
//   score  → スコア順
//   combo  → コンボ順
//
// 例:
//   /api/scores/top10
//   /api/scores/top10?sort=score
//   /api/scores/top10?sort=combo
// ==================================================

app.MapGet(
    "/api/scores/top10",
    (string? sort, ScoreRepository repository) =>
{
    // ==============================================
    // sort未指定の場合はscore
    // ==============================================

    sort = string.IsNullOrWhiteSpace(sort)
        ? "score"
        : sort.ToLowerInvariant();


    // ==============================================
    // ランキング取得
    // ==============================================

    List<ScoreEntry> scores;

    switch (sort)
    {
        // スコア順
        case "score":

            scores =
                repository.GetRanking(
                    rankingMaxEntries
                );

            break;


        // コンボ順
        case "combo":

            scores =
                repository.GetComboRanking(
                    rankingMaxEntries
                );

            break;


        // 不正なsort
        default:

            return Results.BadRequest(new
            {
                error = "Invalid sort parameter.",

                allowed = new[]
                {
                    "score",
                    "combo"
                }
            });
    }


    // ==============================================
    // レスポンス作成
    // ==============================================

    var result =
        scores
            .Select((score, index) => new
            {
                Rank = index + 1,

                score.Player,
                score.Score,
                score.Hit,
                score.Miss,
                score.Combo,
                score.Timestamp,
                score.Version
            })
            .ToList();


    return Results.Ok(result);
});


// ==================================================
// 指定順位のスコア取得
//
// GET /api/scores?rank=1
// ==================================================

app.MapGet(
    "/api/scores",
    (int rank, ScoreRepository repository) =>
{
    // ==============================================
    // rank検証
    // ==============================================

    if (rank <= 0)
    {
        return Results.BadRequest(new
        {
            error = "rank must be 1 or greater."
        });
    }


    // ==============================================
    // ランキング取得
    // ==============================================

    var ranking =
        repository.GetRanking(
            rankingMaxEntries
        );


    // ==============================================
    // 指定順位存在確認
    // ==============================================

    if (rank > ranking.Count)
    {
        return Results.NotFound(new
        {
            error =
                $"Rank {rank} was not found."
        });
    }


    var score =
        ranking[rank - 1];


    // ==============================================
    // レスポンス
    // ==============================================

    return Results.Ok(new
    {
        Rank = rank,

        score.Player,
        score.Score,
        score.Hit,
        score.Miss,
        score.Combo,
        score.Timestamp,
        score.Version
    });
});


// ==================================================
// 隠しコマンド
// 指定順位のデータを削除
//
// DELETE /api/scores?rank=1
// ==================================================

app.MapDelete(
    "/api/scores",
    (int rank, ScoreRepository repository) =>
{
    // ==============================================
    // rank検証
    // ==============================================

    if (rank <= 0)
    {
        return Results.BadRequest(new
        {
            error = "rank must be 1 or greater."
        });
    }


    // ==============================================
    // 指定順位削除
    // ==============================================

    if (!repository.DeleteByRank(
        rank,
        rankingMaxEntries,
        out var deletedEntry))
    {
        return Results.NotFound(new
        {
            error =
                $"Rank {rank} was not found."
        });
    }


    // ==============================================
    // 削除後ランキング
    // ==============================================

    var ranking =
        repository
            .GetRanking(
                rankingMaxEntries
            )
            .Select((score, index) => new
            {
                Rank = index + 1,

                score.Player,
                score.Score,
                score.Hit,
                score.Miss,
                score.Combo,
                score.Timestamp,
                score.Version
            })
            .ToList();


    // ==============================================
    // レスポンス
    // ==============================================

    return Results.Ok(new
    {
        success = true,

        deletedRank = rank,
        deleted = deletedEntry,

        ranking
    });
});


// ==================================================
// サーバー起動
// ==================================================

app.Run();