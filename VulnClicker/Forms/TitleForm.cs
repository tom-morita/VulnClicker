using Microsoft.Win32;
using System.Drawing;
using VulnClicker.Models;

namespace VulnClicker.Forms;

public partial class TitleForm : Form
{
    private readonly StartUpInfo startUpInfo;
    private readonly StartOptions startOptions;

    // ==================================================
    // コンストラクタ
    // ==================================================

    public TitleForm(StartOptions startOptions)
    {
        InitializeComponent();
        this.startOptions = startOptions;
        startUpInfo = StartUpInfoManager.UpdateStartUpInformation();

        // 
        // タイトルテーマ取得
        // 

        TitleTheme theme = GetTitleTheme(startUpInfo.Today);
        ApplyTitleTheme(theme);
        UpdateActivationDisplay();

        // 
        // 起動情報
        // 

        if (startUpInfo.IsFirstStartUp)
        {
            startUpInfoLabel.Text =
                "初めての方はプレイ時間が2倍!!\n" +
                $"今    日: {startUpInfo.Today:yyyy/MM/dd}\n" +
                $"起動回数: {startUpInfo.StartUpCount}";
        }
        else
        {
            startUpInfoLabel.Text =
                $"前回起動: {startUpInfo.StartUpDate:yyyy/MM/dd}\n" +
                $"今    日: {startUpInfo.Today:yyyy/MM/dd}\n" +
                $"起動回数: {startUpInfo.StartUpCount}";
        }

        // 
        // New Game
        // 
        newGameButton.Click += (_, _) =>
        {
            Hide();
            using var game = new Clicker(false, startUpInfo, startOptions);
            game.ShowDialog();
            UpdateActivationDisplay();
            Show();
        };

        // 
        // Load Game
        // 
        loadGameButton.Click += (_, _) =>
        {
            Hide();
            using var game = new Clicker(true, startUpInfo, startOptions);
            game.ShowDialog();
            UpdateActivationDisplay();
            Show();
        };

        // 
        // Import Data
        // 
        importDataButton.Click += (_, _) =>
        {
            Hide();
            using var game = new Clicker(true, startUpInfo, startOptions);
            game.ShowDialog();
            UpdateActivationDisplay();
            Show();
        };

        // 
        // rankingButton
        // 
        rankingButton.Click += async (_, _) =>
        {
            using var game = new Clicker(false, startUpInfo, startOptions);
            await game.ShowRankingAsync();
        };

        // 
        // activationButton
        // 
        activationButton.Click += (_, _) =>
        {
            using var activation = new Activation(startOptions);

            if (activation.ShowDialog(this) == DialogResult.OK)
            {
                UpdateActivationDisplay();
            }
        };

    }

    // ==================================================
    // タイトルテーマ
    // ==================================================
    private static TitleTheme GetTitleTheme(
        DateTime date)
    {
        // 
        // ハロウィン
        // 

        if (date.Month == 10 &&
            (date.Day == 30 ||
             date.Day == 31))
        {
            return new TitleTheme
            {
                Title =
                    "🎃 VULN CLICKER 🎃",

                BackColor =
                    Color.FromArgb(
                        38,
                        24,
                        45
                    ),

                TitleColor =
                    Color.FromArgb(
                        255,
                        140,
                        0
                    ),

                ButtonBackColor =
                    Color.FromArgb(
                        255,
                        102,
                        0
                    ),

                ButtonForeColor =
                    Color.White
            };
        }

        // 
        // クリスマス
        // 

        if (date.Month == 12 &&
            (date.Day == 24 ||
             date.Day == 25))
        {
            return new TitleTheme
            {
                Title =
                    "🎄 VULN CLICKER 🎄",

                BackColor =
                    Color.FromArgb(
                        158,
                        188,
                        158
                    ),

                TitleColor =
                    Color.FromArgb(
                        255,
                        248,
                        232
                    ),

                ButtonBackColor =
                    Color.FromArgb(
                        216,
                        58,
                        86
                    ),

                ButtonForeColor =
                    Color.White
            };
        }

        // 
        // 通常
        // 

        return new TitleTheme
        {
            Title =
                "👾 VULN CLICKER 👾",

            BackColor =
                Color.FromArgb(
                    245,
                    247,
                    250
                ),

            TitleColor =
                Color.FromArgb(
                    31,
                    41,
                    55
                ),

            ButtonBackColor =
                Color.FromArgb(
                    59,
                    130,
                    246
                ),

            ButtonForeColor =
                Color.White
        };
    }
    private void ApplyTitleTheme(TitleTheme theme)
    {
        // フォーム
        BackColor = theme.BackColor;

        // タイトル
        titleLabel.Text = theme.Title;
        titleLabel.ForeColor = theme.TitleColor;

        // 起動情報
        startUpInfoLabel.ForeColor = theme.TitleColor;

        // ボタン
        Button[] buttons =
        {
            newGameButton,
            loadGameButton,
            importDataButton,
            rankingButton,
            activationButton
        };

        foreach (Button button in buttons)
        {
            button.BackColor = theme.ButtonBackColor;
            button.ForeColor = theme.ButtonForeColor;
        }
    }
    private void UpdateActivationDisplay()
    {
        Text = "Vuln Clicker";
        if (Activation.IsActivated)
        {
            Text = $"{Text}（商用版）";
        }
        if (startOptions.Profile == RuntimeProfile.Diagnostic)
        {
            Text += " [DEBUG]";
        }
    }

}