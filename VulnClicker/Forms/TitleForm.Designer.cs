using System.Drawing;
using System.Windows.Forms;

namespace VulnClicker.Forms;

partial class TitleForm
{
    private System.ComponentModel.IContainer components = null;
    private Label titleLabel;
    private Label startUpInfoLabel;
    private Button newGameButton;
    private Button loadGameButton;
    private Button importDataButton;
    private Button rankingButton;
    private Button activationButton;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        titleLabel = new Label();
        startUpInfoLabel = new Label();

        newGameButton = new Button();
        loadGameButton = new Button();
        importDataButton = new Button();
        rankingButton = new Button();
        activationButton = new Button();

        SuspendLayout();

        //
        // titleLabel
        //
        titleLabel.Name = "titleLabel";
        titleLabel.Text = "VULN CLICKER";
        titleLabel.TextAlign = ContentAlignment.MiddleCenter;
        titleLabel.Font = new Font("Consolas", 20F, FontStyle.Bold);
        titleLabel.Location = new Point(0, 20);
        titleLabel.Size = new Size(500, 80);
        titleLabel.AutoSize = false;

        //
        // startUpInfoLabel
        //
        startUpInfoLabel.Name = "startUpInfoLabel";
        startUpInfoLabel.Text = "start up info";
        startUpInfoLabel.TextAlign = ContentAlignment.MiddleCenter;
        startUpInfoLabel.Font = new Font("Consolas", 10F);
        startUpInfoLabel.Location = new Point(50, 125);
        startUpInfoLabel.Size = new Size(400, 70);

        //
        // newGameButton
        //
        newGameButton.Name = "newGameButton";
        newGameButton.Text = "New Game";
        newGameButton.Location = new Point(150, 210);
        newGameButton.TabIndex = 1;

        //
        // loadGameButton
        //
        loadGameButton.Name = "loadGameButton";
        loadGameButton.Text = "Load Game";
        loadGameButton.Location = new Point(150, 270);
        loadGameButton.TabIndex = 2;

        //
        // importDataButton
        //
        importDataButton.Name = "importDataButton";
        importDataButton.Text = "Import Data";
        importDataButton.Location = new Point(150, 330);
        importDataButton.TabIndex = 3;

        //
        // rankingButton
        //
        rankingButton.Name = "rankingButton";
        rankingButton.Text = "Ranking";
        rankingButton.Location = new Point(150, 390);
        rankingButton.TabIndex = 4;

        //
        // activationButton
        //
        activationButton.Name = "activationButton";
        activationButton.Text = "Activation";
        activationButton.Location = new Point(150, 450);
        activationButton.TabIndex = 5;

        //
        // ボタン共通設定
        //
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
            button.Size = new Size(200, 50);
            button.Font = new Font("Consolas", 12F);
            button.TextAlign = ContentAlignment.MiddleCenter;

            Controls.Add(button);
        }

        //
        // TitleForm
        //
        ClientSize = new Size(500, 580);

        Controls.Add(titleLabel);
        Controls.Add(startUpInfoLabel);

        Font = new Font("Consolas", 11F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "TitleForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "VULN CLICKER";

        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}