using Microsoft.Win32;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Media;
using VulnClicker.Services;
using VulnClicker.Models;
using VulnClicker;

namespace VulnClicker.Forms;

public partial class Clicker : Form
{
    // ==================================================
    // ゲーム設定
    // ==================================================

    private const string GameVersion = "1.0.0";
    private const string RegistryPath = @"Software\VulnClicker";
    private const string ScoreRegistryName = "StatsScore";
    private const string HitRegistryName   = "StatsHit";
    private const string MissRegistryName  = "StatsMiss";
    private const string ComboRegistryName = "StatsCombo";
    private const string RemainingTimeRegistryName = "StatsRemainTime";
    private const string HashRegistryName = "StatsHash";

    private string playerName = "anonymous";

    private readonly StartUpInfo startUpInfo;
    private readonly StartOptions startOptions;

    // ==================================================
    // ゲーム状態
    // ==================================================

    private int score = 0;
    private int plusCount = 0;
    private int minusCount = 0;
    private int comboCount = 0;
    // +1対象
    private string plusShape = "〇";
    private Color plusColor = Color.Blue;

    // -1対象
    private string minusShape = "✕";
    private Color minusColor = Color.Red;

    // ==================================================
    // ランダム
    // ==================================================

    private readonly Random random = new();

    private readonly string[] buttonShapes =
    {
        "〇",
        "✕",
        "△",
        "▢"
    };

    // ==================================================
    // タイマー
    // ==================================================

    private const double defaultGameTimeLimit = 30.0;
    // 残り時間（秒）
    private double remainingTime;
    private double gameStartRemainingTime;

    private readonly Stopwatch gameStopwatch = new();

    private System.Windows.Forms.Timer gameTimer;

    private GameState gameState = GameState.Ready;

    // ==================================================
    // サウンド
    // ==================================================

    private bool soundEnabled = true;
    private bool alwaysOnTop = false;

    // ==================================================
    // サーバー
    // ==================================================

    private readonly ScoreClient scoreClient;
    

    // ==================================================
    // コンストラクタ
    // ==================================================

    public Clicker(bool loadGame, StartUpInfo startUpInfo, StartOptions startOptions)
    {
        InitializeComponent();
        this.startUpInfo = startUpInfo;
        this.startOptions = startOptions;
        soundEnabled = startOptions.SoundEnabled;
        alwaysOnTop = startOptions.AlwaysOnTop;
        scoreClient = new ScoreClient(startOptions);
        UpdateActivationDisplay();

        gameTimer = new System.Windows.Forms.Timer();

        // 小数点以下2桁表示に対応するため、
        // 10ms間隔で表示を更新する
        gameTimer.Interval = 10;
        gameTimer.Tick += GameTimer_Tick;

        InitializeGame();

        if (loadGame)
        {
            LoadGame();
        }
    }

    // ==================================================
    // 初期化
    // ==================================================

    private void InitializeGame()
    {
        score = 0;
        plusCount = 0;
        minusCount = 0;

        if (startUpInfo.IsFirstStartUp)
        {
            gameStartRemainingTime = defaultGameTimeLimit * 2;
            remainingTime = gameStartRemainingTime;
        }
        else
        {
            gameStartRemainingTime = defaultGameTimeLimit;
            remainingTime = gameStartRemainingTime;
        }        

        gameState = GameState.Ready;

        // Stopwatchも停止・リセット
        gameStopwatch.Reset();

        // Ready状態なのでボタンは無効
        plusButton.Enabled = false;
        minusButton.Enabled = false;

        UpdateTimeDisplay();

        scorePanel.Location = new Point(
            ClientSize.Width - scorePanel.Width - 20,
            ClientSize.Height - scorePanel.Height - 20);

        // ここではゲームを開始しない
        // ゲーム開始はスコアパネルクリック時

        RandomizeButtonAppearance();
        UpdateScoreDisplay();
        MoveButtons();
        UpdateGameControlButton();
    }

    // ==================================================
    // +1ボタン
    // ==================================================

    private void plusButton_Click(object? sender, EventArgs e)
    {
        // ゲーム中以外は反応しない
        if (gameState != GameState.Playing)
        {
            return;
        }

        plusCount++;
        comboCount++;
        int comboBonus = comboCount - 1;
        score += 1 + comboBonus;

        if (soundEnabled)
        {
            SystemSounds.Asterisk.Play();
        }
        RandomizeTargetLabelPosition();
        UpdateScoreDisplay();
        SaveGame();
        RandomizeButtonAppearance();
        MoveButtons();
    }

    // ==================================================
    // -1ボタン
    // ==================================================

    private void minusButton_Click(object? sender, EventArgs e)
    {
        // ゲーム中以外は反応しない
        if (gameState != GameState.Playing)
        {
            return;
        }

        score--;
        minusCount++;
        comboCount = 0;

        if (soundEnabled)
        {
            SystemSounds.Beep.Play();
        }
        RandomizeTargetLabelPosition();
        UpdateScoreDisplay();
        SaveGame();
        RandomizeButtonAppearance();
        MoveButtons();
    }

    // ==================================================
    // スコア表示
    // ==================================================

    private void UpdateScoreDisplay()
    {
        scoreValueLabel.Text = score.ToString();

        statsValueLabel.Text =
            $"combo:{comboCount}\r\nminus:{minusCount}\r\nacc:{(plusCount + minusCount > 0 ? (double)(plusCount) / (plusCount + minusCount) * 100 : 0):F2}%";
    }

    // ==================================================
    // ゲームタイマー
    // ==================================================

    private void GameTimer_Tick(object? sender, EventArgs e)
    {
        // Playing状態以外ではタイマー処理をしない
        if (gameState != GameState.Playing)
        {
            return;
        }

        remainingTime =
            Math.Max(
                0.0,
                gameStartRemainingTime - gameStopwatch.Elapsed.TotalSeconds);

        UpdateTimeDisplay();

        // 時間切れ
        if (remainingTime <= 0.0)
        {
            remainingTime = 0.0;

            gameStopwatch.Stop();
            gameTimer.Stop();

            gameState = GameState.Finished;

            plusButton.Enabled = false;
            minusButton.Enabled = false;

            UpdateTimeDisplay();
            EndGame();
            UpdateTimeDisplay();
            UpdateGameControlButton();

            // 残り時間 0 をレジストリへ保存
            SaveGame();

            // 結果入力画面
            //using RegisterResultForm saveResultForm = new RegisterResultForm(
            //    score,
            //    missCount);

            //saveResultForm.ShowDialog(this);            
        }
    }

    // ==================================================
    // ゲーム終了
    // ==================================================

    private void EndGame()
    {
        gameTimer.Stop();
        gameStopwatch.Stop();

        gameState = GameState.Finished;

        plusButton.Enabled = false;
        minusButton.Enabled = false;

        UpdateTimeDisplay();

        // プレイヤー名入力フォームを表示
        using var nameForm = new RegisterResultForm(
            playerName,
            score,
            plusCount,
            minusCount,
            comboCount,
            (int)Math.Floor(remainingTime));
            //RegisterResultForm();

        if (nameForm.ShowDialog(this) == DialogResult.OK)
        {
            playerName = nameForm.PlayerName;

            // ハイスコア登録など
        }
    }

    // ==================================================
    // 時間表示
    // ==================================================

    private void UpdateTimeDisplay()
    {
        timeValueLabel.Text =
            $"{gameState}:{remainingTime:000.00}";
    }

    // ==================================================
    // スコアパネルクリック
    //
    // Ready   → Playing
    // Playing → Paused
    // Paused  → Playing
    // Finished → No Operation
    // ==================================================


    private void gameControlButton_Click(object? sender, EventArgs e)
    {
        switch (gameState)
        {
            case GameState.Ready:
                // Ready → Playing
                gameState = GameState.Playing;

                // 現在の残り時間を、今回のゲーム開始時点の基準時間として保存
                // New Gameなら30秒/60秒、Load Gameならレジストリから読み込んだ時間
                gameStartRemainingTime = remainingTime;

                // ゲーム開始時点からの経過時間を計測
                gameStopwatch.Reset();
                gameStopwatch.Start();

                plusButton.Enabled = true;
                minusButton.Enabled = true;

                gameTimer.Start();

                UpdateTimeDisplay();
                UpdateGameControlButton();
                break;

            case GameState.Playing:
                // Playing → Paused
                gameState = GameState.Paused;

                gameStopwatch.Stop();
                gameTimer.Stop();

                plusButton.Enabled = false;
                minusButton.Enabled = false;

                UpdateTimeDisplay();
                UpdateGameControlButton();
                break;

            case GameState.Paused:
                // Paused → Playing
                gameState = GameState.Playing;

                // Stopwatchは一時停止時点から再開する
                gameStopwatch.Start();
                gameTimer.Start();

                plusButton.Enabled = true;
                minusButton.Enabled = true;

                UpdateTimeDisplay();
                UpdateGameControlButton();
                break;

            case GameState.Finished:
                // Finishedでは何もしない
                break;
        }
    }

    private void UpdateGameControlButton()
    {
        switch (gameState)
        {
            case GameState.Ready:
                gameControlButton.Text = "▶";
                gameControlButton.Enabled = true;
                break;

            case GameState.Playing:
                gameControlButton.Text = "⏸";
                gameControlButton.Enabled = true;
                break;

            case GameState.Paused:
                gameControlButton.Text = "▶";
                gameControlButton.Enabled = true;
                break;

            case GameState.Finished:
                gameControlButton.Text = "-";
                gameControlButton.Enabled = false;
                break;
        }
    }

    // ==================================================
    // ボタンの柄・色をランダム化
    // ==================================================

    private void RandomizeButtonAppearance()
    {
        // =========================
        // 文字をランダム設定
        // =========================

        string newPlusShape =
            buttonShapes[random.Next(buttonShapes.Length)];

        string newMinusShape =
            buttonShapes[random.Next(buttonShapes.Length)];

        // 文字が重複しないようにする
        while (newMinusShape == newPlusShape)
        {
            newMinusShape =
                buttonShapes[random.Next(buttonShapes.Length)];
        }

        // =========================
        // 色をランダム設定
        // =========================

        Color[] buttonColors =
        {
            Color.Blue,
            Color.Red,
            Color.Green,
            Color.Orange,
            Color.Purple,
            Color.Teal
        };

        Color newPlusColor =
            buttonColors[random.Next(buttonColors.Length)];

        Color newMinusColor =
            buttonColors[random.Next(buttonColors.Length)];

        // 色が重複しないようにする
        while (newMinusColor == newPlusColor)
        {
            newMinusColor =
                buttonColors[random.Next(buttonColors.Length)];
        }

        // フィールドにも保存
        plusShape = newPlusShape;
        minusShape = newMinusShape;

        plusColor = newPlusColor;
        minusColor = newMinusColor;

        // =========================
        // ボタンへ反映
        // =========================

        plusButton.Text = plusShape;
        plusButton.BackColor = plusColor;
        plusButton.ForeColor = Color.White;
        plusButton.Font = new Font(
            "Consolas",
            20F,
            FontStyle.Bold);

        plusButton.TextAlign =
            ContentAlignment.MiddleCenter;

        minusButton.Text = minusShape;
        minusButton.BackColor = minusColor;
        minusButton.ForeColor = Color.White;
        minusButton.Font = new Font(
            "Consolas",
            20F,
            FontStyle.Bold);

        minusButton.TextAlign =
            ContentAlignment.MiddleCenter;

        // ターゲット表示も更新
        plusTargetLabel.Text =
            $"+1 : {plusShape}";

        minusTargetLabel.Text =
            $"-1 : {minusShape}";

        plusTargetLabel.ForeColor =
            plusColor;

        minusTargetLabel.ForeColor =
            minusColor;
    }

    // ==================================================
    // ボタン配置
    // ==================================================

    private void MoveButtons()
    {
        plusButton.Location =
            RandomLocation(
                plusButton.Size,
                minusButton);

        minusButton.Location =
            RandomLocation(
                minusButton.Size,
                plusButton);
    }

    // ==================================================
    // ランダム位置
    // ==================================================

    private Point RandomLocation(
        Size buttonSize,
        Control otherButton)
    {
        const int margin = 20;

        int topLimit =
            menuStrip.Bottom + margin;

        int bottomLimit =
            scorePanel.Top - margin;

        int availableWidth =
            ClientSize.Width -
            buttonSize.Width -
            margin * 2;

        int availableHeight =
            bottomLimit -
            topLimit -
            buttonSize.Height;

        if (availableWidth <= 0)
        {
            return new Point(
                margin,
                topLimit);
        }

        if (availableHeight <= 0)
        {
            return new Point(
                margin,
                topLimit);
        }

        for (int i = 0; i < 100; i++)
        {
            int x =
                random.Next(
                    margin,
                    margin + availableWidth + 1);

            int y =
                random.Next(
                    topLimit,
                    topLimit + availableHeight + 1);

            Rectangle candidate =
                new Rectangle(
                    x,
                    y,
                    buttonSize.Width,
                    buttonSize.Height);

            Rectangle other =
                new Rectangle(
                    otherButton.Location,
                    otherButton.Size);

            if (!candidate.IntersectsWith(other))
            {
                return new Point(x, y);
            }
        }

        return new Point(
            margin,
            topLimit);
    }

    // ==================================================
    // New Game
    // ==================================================

    private void newGameMenuItem_Click(
        object? sender,
        EventArgs e)
    {
        NewGame();
    }

    private void NewGame()
    {
        // ゲームタイマーを停止
        gameTimer.Stop();

        // Stopwatchをリセット
        gameStopwatch.Reset();

        score = 0;
        plusCount = 0;
        minusCount = 0;
        comboCount = 0;

        // ゲーム開始前に戻す
        gameState = GameState.Ready;

        // Readyなのでボタンは無効
        plusButton.Enabled = false;
        minusButton.Enabled = false;

        UpdateScoreDisplay();
        UpdateTimeDisplay();
        RandomizeButtonAppearance();
        MoveButtons();
        UpdateGameControlButton();
        SaveGame();
    }

    // ==================================================
    // Load Game
    // ==================================================

    private void loadGameMenuItem_Click(
        object? sender,
        EventArgs e)
    {
        LoadGame();
    }

    private void LoadGame()
    {
        try
        {
            using RegistryKey? key =
                Registry.CurrentUser.OpenSubKey(
                    RegistryPath);

            if (key == null)
            {
                MessageBox.Show(
                    "保存されたゲームデータがありません。",
                    "Load Failure",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            object? remainingTimeValue =
                key.GetValue(RemainingTimeRegistryName);

            object? scoreValue =
                key.GetValue(ScoreRegistryName);

            object? hitValue =
                key.GetValue(HitRegistryName);

            object? missValue =
                key.GetValue(MissRegistryName);

            object? comboValue =
                key.GetValue(ComboRegistryName);

            object? storedHash =
                key.GetValue(HashRegistryName);

            if (
                remainingTimeValue == null ||
                scoreValue == null ||
                hitValue == null ||
                missValue == null ||
                comboValue == null ||
                storedHash == null)
            {
                MessageBox.Show(
                    "ゲームデータが不完全です。\r\n新規ゲームを開始します。",
                    "Load Failure",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            int loadedRemainingTime = Convert.ToInt32(remainingTimeValue);
            int loadedScore =         Convert.ToInt32(scoreValue);
            int loadedHit =           Convert.ToInt32(hitValue);
            int loadedMiss =          Convert.ToInt32(missValue);
            int loadedCombo =         Convert.ToInt32(comboValue);

            string expectedHash =
                CreateStatsHash(
                    loadedRemainingTime,
                    loadedScore,
                    loadedHit,
                    loadedMiss,
                    loadedCombo
                );

            if (loadedRemainingTime <= 0 )
            {
                MessageBox.Show(
                    "残り時間がありません。新しいゲームを始めてください。",
                    "Load",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }
            
            if (!string.Equals(
                    expectedHash,
                    storedHash.ToString(),
                    StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    "ゲームデータの整合性を確認できませんでした。",
                    "Load Failure",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }
            else
            {
                MessageBox.Show(
                    "前回のゲームデータを読み込みました。",
                    "Load Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            remainingTime = (double)loadedRemainingTime;
            score         = loadedScore;
            plusCount     = loadedHit;
            minusCount    = loadedMiss;
            comboCount    = loadedCombo;

            UpdateScoreDisplay();
            UpdateTimeDisplay();
            RandomizeButtonAppearance();
            MoveButtons();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"ゲームデータの読み込みに失敗しました。\n\n{ex.Message}",
                "Load Failure",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    // ==================================================
    // Activation
    // ==================================================

    private void UpdateActivationDisplay()
    {
        Text = "Vuln Clicker";
        if (Activation.IsActivated)
        {
            Text = "Vuln Clicker（商用版）";
        }
        else
        {
            Text = Text;
        }
        if (startOptions.Profile == RuntimeProfile.Diagnostic)
        {
            Text += " [DEBUG]";
        }
    }

    private void activationMenuItem_Click(
        object? sender,
        EventArgs e)
    {
        using var activation = new Activation(startOptions);
        if (activation.ShowDialog(this) == DialogResult.OK)
        {
            UpdateActivationDisplay();
        }
    }

    // ==================================================
    // Back to Title
    // ==================================================

    private void backToTitleMenuItem_Click(object? sender, EventArgs e)
    {
        gameTimer.Stop();
        gameStopwatch.Stop();

        Close();

    }

    // ==================================================
    // Reset
    // ==================================================

    private void resetMenuItem_Click(
        object? sender,
        EventArgs e)
    {
        DialogResult result =
            MessageBox.Show(
                "スコアを0に戻しますか？",
                "Reset",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

        if (result != DialogResult.Yes)
        {
            return;
        }

        score = 0;
        plusCount = 0;
        minusCount = 0;

        UpdateScoreDisplay();
        SaveGame();
    }

    // ==================================================
    // 保存
    // ==================================================

    private void SaveGame()
    {
        try
        {
            using RegistryKey key =
                Registry.CurrentUser.CreateSubKey(
                    RegistryPath);

            if (key == null)
            {
                return;
            }

            int remainingTimeFloor = (int)Math.Floor(remainingTime);

            key.SetValue(
                RemainingTimeRegistryName,
                remainingTimeFloor,
                RegistryValueKind.DWord);

            key.SetValue(
                ScoreRegistryName,
                score,
                RegistryValueKind.DWord);

            key.SetValue(
                HitRegistryName,
                plusCount,
                RegistryValueKind.DWord);

            key.SetValue(
                MissRegistryName,
                minusCount,
                RegistryValueKind.DWord);

            key.SetValue(
                ComboRegistryName,
                comboCount,
                RegistryValueKind.DWord);

            key.SetValue(
                HashRegistryName,
                CreateStatsHash(
                    remainingTimeFloor,
                    score,
                    plusCount,
                    minusCount,
                    comboCount
                    ),
                RegistryValueKind.String);
        }
        catch
        {
            // 保存できない場合でもゲーム自体は継続する
        }
    }

    // ==================================================
    // ハッシュ生成
    // ==================================================

    private string CreateStatsHash(
        int currentRemainingTime,
        int currentScore,
        int currentHit,
        int currentMiss,
        int currentCombo)
    {
        string source = $"VulnClicker|{GameVersion}|{currentRemainingTime}|{currentScore}|{currentHit}|{currentMiss}|{currentCombo}";
        byte[] bytes = Encoding.UTF8.GetBytes(source);
        byte[] hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }

    // ==================================================
    // Export Data
    // ==================================================

    private void exportFileMenuItem_Click(
        object? sender,
        EventArgs e)
    {
        using SaveFileDialog dialog = new()
        {
            Title = "セーブデータのエクスポート",
            Filter = "VulnClicker Save Data (*.dat)|*.dat",
            DefaultExt = "dat",
            AddExtension = true,
            FileName = "VulnClickerSave.dat"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            GameSaveData saveData = new()
            {
                Player = playerName,
                Score = score,
                Hit = plusCount,
                Miss = minusCount,
                Combo = comboCount,
                RemainingTime = (int)Math.Floor(remainingTime)
            };

            ExportData.Export(
                dialog.FileName,
                saveData);

            MessageBox.Show(
                "セーブデータをエクスポートしました。",
                "Export Data",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"エクスポートに失敗しました。\n\n{ex.Message}",
                "Export Data",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
    private void importFileMenuItem_Click(
        object? sender,
        EventArgs e)
    {
        using OpenFileDialog dialog = new()
        {
            Title = "セーブデータのインポート",
            Filter = "VulnClicker Save Data (*.dat)|*.dat",
            CheckFileExists = true
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            GameSaveData saveData = ImportData.Import(dialog.FileName);
            playerName = saveData.Player;
            remainingTime = saveData.RemainingTime;
            gameStartRemainingTime = remainingTime;
            score = saveData.Score;
            plusCount = saveData.Hit;
            minusCount = saveData.Miss;
            comboCount = saveData.Combo;

            if (saveData.RemainingTime <= 0)
            {
                MessageBox.Show(
                    "残り時間がありません。",
                    "Import Data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                //return;
            }
            using var resultForm =
                new RegisterResultForm(
                    saveData.Player,
                    saveData.Score,
                    saveData.Hit,
                    saveData.Miss,
                    saveData.Combo,
                    saveData.RemainingTime);

            if (resultForm.ShowDialog(this)
                != DialogResult.OK)
    {
                // NOを押した場合は何も反映しない
                return;
            }
            // ゲームを停止
            gameTimer.Stop();
            gameStopwatch.Reset();

            // 読み込んだデータを反映
            playerName = resultForm.PlayerName;
            remainingTime = saveData.RemainingTime;
            gameStartRemainingTime = remainingTime;

            score = saveData.Score;
            plusCount = saveData.Hit;
            minusCount = saveData.Miss;
            comboCount = saveData.Combo;

            // Ready状態へ
            gameState = GameState.Ready;


            plusButton.Enabled = false;
            minusButton.Enabled = false;

            UpdateScoreDisplay();
            UpdateTimeDisplay();
            UpdateGameControlButton();

            RandomizeButtonAppearance();
            MoveButtons();

            // Registryにも反映
            SaveGame();

            MessageBox.Show(
                $"セーブデータをインポートしました。\r\n" +
                $"Player: {saveData.Player}\r\n" +
                $"Score: {saveData.Score}\r\n" +
                $"Hit: {saveData.Hit}\r\n" +
                $"Miss: {saveData.Miss}\r\n" +
                $"Combo: {saveData.Combo}\r\n" +
                $"Remaining Time: {saveData.RemainingTime}",
                "Import Data",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (FormatException)
        {
            MessageBox.Show(
                "セーブデータのBase64形式が正しくありません。",
                "Import Data",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"セーブデータの読み込みに失敗しました。\n\n{ex.Message}",
                "Import Data",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    // ==================================================
    // Sound
    // ==================================================

    private void soundMenuItem_Click(
        object? sender,
        EventArgs e)
    {
        soundEnabled = soundMenuItem.Checked;
    }

    // ==================================================
    // Always On Top
    // ==================================================

    private void alwaysOnTopMenuItem_Click(
        object? sender,
        EventArgs e)
    {
        TopMost =
            alwaysOnTopMenuItem.Checked;
    }

    // ==================================================
    // Exit
    // ==================================================

    private void exitMenuItem_Click(
        object? sender,
        EventArgs e)
    {
        Close();
    }

    // ==================================================
    // スコア登録
    // ==================================================

    private async void registerScoreMenuItem_Click(
        object? sender,
        EventArgs e)
    {
        using var resultForm =
            new RegisterResultForm(
                playerName,
                score,
                plusCount,
                minusCount,
                comboCount,
                (int)Math.Floor(remainingTime));

        if (resultForm.ShowDialog(this)
            != DialogResult.OK)
        {
            return;
        }

        playerName = resultForm.PlayerName;
        registerScoreMenuItem.Enabled = false;

        try
        {
            //string playerName = Environment.UserName;
            //string playerName = "anonymous";
            playerName = resultForm.PlayerName;

            var scoreEntry =
                new ScoreEntry
                {
                    Version = GameVersion,
                    Timestamp = DateTimeOffset.Now,
                    Player = playerName,
                    Score = score,
                    Hit = plusCount,
                    Miss = minusCount,
                    Combo = comboCount,
                };

            bool success =
                await scoreClient.RegisterScoreAsync(
                    scoreEntry);

            if (success)
            {
                MessageBox.Show(
                    "スコアを登録しました。",
                    "Ranking",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "スコアを登録できませんでした。\n" +
                    "スコアサーバーが起動しているか確認してください。",
                    "Online Ranking",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"スコア登録中にエラーが発生しました。\n\n{ex.Message}",
                "Online Ranking",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            registerScoreMenuItem.Enabled = true;
        }
    }

    // ==================================================
    // ランキング表示
    // ==================================================
    public async Task ShowRankingAsync()
    {
        try
        {
            List<RankingEntry> ranking =
                await scoreClient.GetTop10Async();

            if (ranking.Count == 0)
            {
                MessageBox.Show(
                    "オンラインランキングデータがありません。\r\n" +
                    "スコアサーバーが起動しているか確認してください。",
                    "Online Ranking Failure",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            StringBuilder text = new();

            text.AppendLine("===== Vuln Clicker Ranking =====");
            text.AppendLine();

            foreach (RankingEntry entry in ranking)
            {
                text.AppendLine(
                    $"{entry.Rank,2}. " +
                    $"{entry.Player,-15} " +
                    $"Score: {entry.Score,6} " +
                    $"Miss: {entry.Miss,4}");
            }

            MessageBox.Show(
                text.ToString(),
                "Online Ranking",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"ランキング取得中にエラーが発生しました。\n\n{ex.Message}",
                "Online Ranking Failure",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private async void showRankingMenuItem_Click(
        object? sender,
        EventArgs e)
    {
        await ShowRankingAsync();
    }

    // ==================================================
    // About
    // ==================================================

    private void aboutMenuItem_Click(
        object? sender,
        EventArgs e)
    {
        MessageBox.Show(
            "VulnClicker\n\n" +
            "Simple Clicker Game\n" +
            $"Version {GameVersion}",
            "About",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
    private void aboutVulnItem_Click(
        object? sender,
        EventArgs e)
    {
        MessageBox.Show(
            "ハック:\n" +
            " - 1:起動回数を1000に改ざんする。初回起動に改ざんする\n" +
            " - 2:シリアルコード入力により製品版に変更する\n" +
            " - 3:スコアを改ざんする（ローカル保存データ）\n" +
            " - 4:スコアを改ざんする（オンライン登録）\n" +
            " - 5:デバッグモードを起動する。\n" +
            "イースターエッグ:\n" +
            " - 1:タイトルのデザインを変更する\n" +
            "チート:\n" +
            " - 1:マウスを一切利用せずスコアを獲得する\n",
            "Hack menu",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
    // ==================================================
    // フォームサイズ変更
    // ==================================================

    protected override void OnResize(
        EventArgs e)
    {
        base.OnResize(e);

        if (scorePanel == null)
        {
            return;
        }

        // スコアパネルを右下に固定
        scorePanel.Location =
            new Point(
                Math.Max(
                    0,
                    ClientSize.Width -
                    scorePanel.Width -
                    20),

                Math.Max(
                    menuStrip.Bottom + 10,
                    ClientSize.Height -
                    scorePanel.Height -
                    20));
    }


}
