#nullable disable

namespace VulnClicker.Forms;

partial class Clicker
{
  /// <summary>
  /// デザイナーで使用するコンポーネント。
  /// </summary>
  private System.ComponentModel.IContainer components = null;

  // =========================
  // メニュー
  // =========================
  private MenuStrip menuStrip;

  private ToolStripMenuItem fileMenuItem;
  private ToolStripMenuItem exportFileMenuItem;
  private ToolStripMenuItem importFileMenuItem;
  private ToolStripMenuItem backToTitleMenuItem;
  private ToolStripMenuItem exitMenuItem;

  private ToolStripMenuItem gameMenuItem;
  private ToolStripMenuItem newGameMenuItem;
  private ToolStripMenuItem loadGameMenuItem;
  private ToolStripMenuItem activationMenuItem;

  private ToolStripMenuItem resetMenuItem;

  private ToolStripMenuItem optionsMenuItem;
  private ToolStripMenuItem soundMenuItem;
  private ToolStripMenuItem alwaysOnTopMenuItem;

  private ToolStripMenuItem rankingMenuItem;
  private ToolStripMenuItem registerScoreMenuItem;
  private ToolStripMenuItem showRankingMenuItem;

  private ToolStripMenuItem helpMenuItem;
  private ToolStripMenuItem aboutMenuItem;
  private ToolStripMenuItem aboutVulnItem;

  // =========================
  // ゲームボタン
  // =========================
  private Button plusButton;
  private Button minusButton;
  private Button gameControlButton;

  // =========================
  // スコア表示
  // =========================
  private Panel scorePanel;
  private Label scoreTitleLabel;
  private Label scoreValueLabel;
  private Label timeTitleLabel;
  private Label timeValueLabel;
  private Label statsTitleLabel;
  private Label statsValueLabel;
  private Label plusTargetLabel;
  private Label minusTargetLabel;


  /// <summary>
  /// 使用中のリソースを解放します。
  /// </summary>
  protected override void Dispose(bool disposing)
  {
    if (disposing)
    {
      components?.Dispose();
    }

    base.Dispose(disposing);
  }

  /// <summary>
  /// UI部品を初期化します。
  /// </summary>
  private void InitializeComponent()
  {

    components = new System.ComponentModel.Container();

    // =========================
    // メニュー
    // =========================

    menuStrip = new MenuStrip();

    fileMenuItem = new ToolStripMenuItem();
    newGameMenuItem = new ToolStripMenuItem();
    loadGameMenuItem = new ToolStripMenuItem();
    activationMenuItem = new ToolStripMenuItem();
    backToTitleMenuItem = new ToolStripMenuItem();
    exitMenuItem = new ToolStripMenuItem();

    gameMenuItem = new ToolStripMenuItem();
    resetMenuItem = new ToolStripMenuItem();

    optionsMenuItem = new ToolStripMenuItem();
    soundMenuItem = new ToolStripMenuItem();
    alwaysOnTopMenuItem = new ToolStripMenuItem();

    rankingMenuItem = new ToolStripMenuItem();
    registerScoreMenuItem = new ToolStripMenuItem();
    showRankingMenuItem = new ToolStripMenuItem();

    helpMenuItem = new ToolStripMenuItem();
    aboutMenuItem = new ToolStripMenuItem();
    aboutVulnItem = new ToolStripMenuItem();

    // =========================
    // ゲームボタン
    // =========================

    plusButton = new Button();
    minusButton = new Button();
    gameControlButton = new Button();

    // =========================
    // スコア表示
    // =========================

    scorePanel = new Panel();
    scoreTitleLabel = new Label();
    scoreValueLabel = new Label();
    timeTitleLabel = new Label();
    timeValueLabel = new Label();
    statsTitleLabel = new Label();
    statsValueLabel = new Label();
    plusTargetLabel = new Label();
    minusTargetLabel = new Label();

    //gameControlButton = new System.Windows.Forms.Button();
    gameControlButton.Click += new System.EventHandler(gameControlButton_Click);

    SuspendLayout();
    menuStrip.SuspendLayout();
    scorePanel.SuspendLayout();

    // ==================================================
    // MenuStrip
    // ==================================================

    menuStrip.Items.AddRange(new ToolStripItem[]
    {
            fileMenuItem,
            gameMenuItem,
            optionsMenuItem,
            rankingMenuItem,
            helpMenuItem
    });

    menuStrip.Location = new Point(0, 0);
    menuStrip.Name = "menuStrip";
    menuStrip.Size = new Size(1000, 24);
    menuStrip.TabIndex = 0;

    // --------------------------------------------------
    // File
    // --------------------------------------------------


    exportFileMenuItem = new ToolStripMenuItem();
    importFileMenuItem = new ToolStripMenuItem();
    fileMenuItem.Name = "fileMenuItem";
    fileMenuItem.Text = "File";

    //
    // exportFileMenuItem
    //
    exportFileMenuItem.Name = "exportFileMenuItem";
    exportFileMenuItem.Size = new Size(180, 22);
    exportFileMenuItem.Text = "Export Data";
    exportFileMenuItem.Click += exportFileMenuItem_Click;

    //
    // importFileMenuItem
    //
    importFileMenuItem.Name = "importFileMenuItem";
    importFileMenuItem.Size = new Size(180, 22);
    importFileMenuItem.Text = "Import Data";
    importFileMenuItem.Click += importFileMenuItem_Click;

    fileMenuItem.DropDownItems.AddRange(new ToolStripItem[]
    {
            exportFileMenuItem,
            importFileMenuItem,
            backToTitleMenuItem,
            new ToolStripSeparator(),
            exitMenuItem
    });

    backToTitleMenuItem.Name = "backToTitleMenuItem";
    backToTitleMenuItem.Text = "Back to Title";
    backToTitleMenuItem.Click += backToTitleMenuItem_Click;

    exitMenuItem.Name = "exitMenuItem";
    exitMenuItem.Text = "Exit";
    exitMenuItem.Click += exitMenuItem_Click;

    // --------------------------------------------------
    // Game
    // --------------------------------------------------

    gameMenuItem.Name = "gameMenuItem";
    gameMenuItem.Text = "Game";

    gameMenuItem.DropDownItems.AddRange(new ToolStripItem[]
    {
            newGameMenuItem,
            loadGameMenuItem,
            activationMenuItem,
    });

    newGameMenuItem.Name = "newGameMenuItem";
    newGameMenuItem.Text = "New Game";
    newGameMenuItem.Click += newGameMenuItem_Click;

    loadGameMenuItem.Name = "loadGameMenuItem";
    loadGameMenuItem.Text = "Load Game";
    loadGameMenuItem.Click += loadGameMenuItem_Click;

    activationMenuItem.Name = "activationMenuItem";
    activationMenuItem.Text = "Activation";
    activationMenuItem.Click += activationMenuItem_Click;
    // --------------------------------------------------
    // Options
    // --------------------------------------------------

    optionsMenuItem.Name = "optionsMenuItem";
    optionsMenuItem.Text = "Options";

    optionsMenuItem.DropDownItems.AddRange(new ToolStripItem[]
    {
            soundMenuItem,
            alwaysOnTopMenuItem
    });

    soundMenuItem.Name = "soundMenuItem";
    soundMenuItem.Text = "Sound";
    soundMenuItem.CheckOnClick = true;
    soundMenuItem.Checked = true;
    soundMenuItem.Click += soundMenuItem_Click;

    alwaysOnTopMenuItem.Name = "alwaysOnTopMenuItem";
    alwaysOnTopMenuItem.Text = "Always On Top";
    alwaysOnTopMenuItem.CheckOnClick = true;
    alwaysOnTopMenuItem.Click += alwaysOnTopMenuItem_Click;

    // --------------------------------------------------
    // Ranking
    // --------------------------------------------------

    rankingMenuItem.Name = "rankingMenuItem";
    rankingMenuItem.Text = "Ranking";

    rankingMenuItem.DropDownItems.AddRange(new ToolStripItem[]
    {
            registerScoreMenuItem,
            showRankingMenuItem
    });

    registerScoreMenuItem.Name = "registerScoreMenuItem";
    registerScoreMenuItem.Text = "Register";
    registerScoreMenuItem.Click += registerScoreMenuItem_Click;

    showRankingMenuItem.Name = "showRankingMenuItem";
    showRankingMenuItem.Text = "Show";
    showRankingMenuItem.Click += showRankingMenuItem_Click;

    // --------------------------------------------------
    // Help
    // --------------------------------------------------

    helpMenuItem.Name = "helpMenuItem";
    helpMenuItem.Text = "Help";

    helpMenuItem.DropDownItems.Add(aboutMenuItem);
    helpMenuItem.DropDownItems.Add(aboutVulnItem);

    aboutMenuItem.Name = "aboutMenuItem";
    aboutMenuItem.Text = "About";
    aboutMenuItem.Click += aboutMenuItem_Click;

    aboutVulnItem.Name = "aboutVulnItem";
    aboutVulnItem.Text = "Vuln Items";
    aboutVulnItem.Click += aboutVulnItem_Click;

    // ==================================================
    // +1 Button
    // ==================================================

    plusButton.Name = "plusButton";
    plusButton.Size = new Size(60, 60);
    plusButton.TabIndex = 3;
    //plusButton.UseVisualStyleBackColor = true;
    plusButton.BackColor = Color.Blue;
    plusButton.ForeColor = Color.White;
    plusButton.Text = "〇";
    plusButton.Font = new Font("Consolas", 30F, FontStyle.Bold);
    plusButton.TextAlign = ContentAlignment.MiddleCenter;

    plusButton.Click += plusButton_Click;

    // ==================================================
    // -1 Button
    // ==================================================

    minusButton.Name = "minusButton";
    minusButton.Size = new Size(60, 60);
    minusButton.TabIndex = 2;
    //minusButton.UseVisualStyleBackColor = true;
    minusButton.BackColor = Color.Red;
    minusButton.ForeColor = Color.White;
    minusButton.Text = "×";
    minusButton.Font = new Font("Consolas", 30F, FontStyle.Bold);
    minusButton.TextAlign = ContentAlignment.MiddleCenter;

    minusButton.Click += minusButton_Click;

    // ==================================================
    // Score Panel
    // ==================================================

    scorePanel.BorderStyle = BorderStyle.FixedSingle;

    scorePanel.Controls.Add(scoreTitleLabel);
    scorePanel.Controls.Add(scoreValueLabel);

    scorePanel.Controls.Add(timeTitleLabel);
    scorePanel.Controls.Add(timeValueLabel);

    scorePanel.Controls.Add(statsTitleLabel);
    scorePanel.Controls.Add(statsValueLabel);
    scorePanel.Controls.Add(plusTargetLabel);
    scorePanel.Controls.Add(minusTargetLabel);

    scorePanel.Controls.Add(gameControlButton);

    scorePanel.Name = "scorePanel";
    scorePanel.Size = new Size(250, 200);
    scorePanel.TabStop = false;
    gameControlButton.TabStop = false;

    // --------------------------------------------------
    // Score title/value
    // --------------------------------------------------

    scoreTitleLabel.AutoSize = false;
    scoreTitleLabel.Font = new Font(
        "Consolas",
        12F,
        FontStyle.Bold,
        GraphicsUnit.Point);
    scoreTitleLabel.Location = new Point(10, 10);
    scoreTitleLabel.Name = "scoreTitleLabel";
    scoreTitleLabel.Size = new Size(80, 30);
    scoreTitleLabel.Text = "SCORE:";
    scoreTitleLabel.TextAlign = ContentAlignment.TopLeft;

    scoreValueLabel.AutoSize = false;
    scoreValueLabel.Font = new Font(
        "Consolas",
        30F,
        FontStyle.Bold,
        GraphicsUnit.Point);
    scoreValueLabel.Location = new Point(90, 10);
    scoreValueLabel.Name = "scoreValueLabel";
    scoreValueLabel.Size = new Size(150, 40);
    scoreValueLabel.Text = "0";
    scoreValueLabel.TextAlign = ContentAlignment.TopRight;

    // --------------------------------------------------
    // Timer title/value
    // --------------------------------------------------

    timeTitleLabel.AutoSize = false;
    timeTitleLabel.Font = new Font(
        "Consolas",
        12F,
        FontStyle.Bold,
        GraphicsUnit.Point);
    timeTitleLabel.Location = new Point(10, 50);
    timeTitleLabel.Name = "timeTitleLabel";
    timeTitleLabel.Size = new Size(80, 30);
    timeTitleLabel.Text = "TIME:";
    timeTitleLabel.TextAlign = ContentAlignment.TopLeft;

    timeValueLabel.AutoSize = false;
    timeValueLabel.Font = new Font(
        "Consolas",
        12F,
        FontStyle.Bold,
        GraphicsUnit.Point);

    timeValueLabel.Location = new Point(90, 50);
    timeValueLabel.Name = "timeValueLabel";
    timeValueLabel.Size = new Size(150, 30);
    timeValueLabel.Text = "30.00";
    timeValueLabel.TextAlign = ContentAlignment.TopRight;

    // --------------------------------------------------
    // STATS title/value
    // --------------------------------------------------

    statsTitleLabel.AutoSize = false;
    statsTitleLabel.Font = new Font(
        "Consolas",
        12F,
        GraphicsUnit.Point);
    statsTitleLabel.Location = new Point(10, 80);
    statsTitleLabel.Name = "statsTitleLabel";
    statsTitleLabel.Size = new Size(80, 30);
    statsTitleLabel.Text = "STATS:";
    statsTitleLabel.TextAlign = ContentAlignment.TopLeft;

    statsValueLabel.AutoSize = false;
    statsValueLabel.Font = new Font(
        "Consolas",
        12F,
        GraphicsUnit.Point);
    statsValueLabel.Location = new Point(90, 80);
    statsValueLabel.Name = "statsValueLabel";
    statsValueLabel.Size = new Size(150, 60);
    statsValueLabel.Text = "combp:0\r\nmiss:0\r\nacc:100.00%";
    statsValueLabel.TextAlign = ContentAlignment.TopRight;

    // --------------------------------------------------
    // +1 target
    // --------------------------------------------------

    plusTargetLabel.AutoSize = false;
    plusTargetLabel.Font = new Font(
        "Consolas",
        18F,
        FontStyle.Bold,
        GraphicsUnit.Point);
    plusTargetLabel.Location = new Point(10, 140);
    plusTargetLabel.Name = "plusTargetLabel";
    plusTargetLabel.Size = new Size(140, 30);
    plusTargetLabel.Text = "+1:〇";
    plusTargetLabel.TextAlign = ContentAlignment.TopLeft;

    plusTargetLabel.ForeColor = Color.Blue;

    // --------------------------------------------------
    // -1 target
    // --------------------------------------------------

    minusTargetLabel.AutoSize = false;
    minusTargetLabel.Font = new Font(
        "Consolas",
        18F,
        FontStyle.Bold,
        GraphicsUnit.Point);
    minusTargetLabel.Location = new Point(10, 170);
    minusTargetLabel.Name = "minusTargetLabel";
    minusTargetLabel.Size = new Size(140, 30);
    minusTargetLabel.Text = "-1:✕";
    minusTargetLabel.TextAlign = ContentAlignment.TopLeft;

    minusTargetLabel.ForeColor = Color.Red;

    // --------------------------------------------------
    // gameControlButton
    // --------------------------------------------------
    gameControlButton.Location = new Point(150, 140);
    gameControlButton.Name = "gameControlButton";
    gameControlButton.Size = new Size(50, 50);
    gameControlButton.Text = "▶";
    gameControlButton.TextAlign = ContentAlignment.MiddleCenter;
    gameControlButton.Font = new Font(
      "Consolas",
      30F,
      FontStyle.Bold,
      GraphicsUnit.Point);


    // ==================================================
    // Clicker Form
    // ==================================================

    AutoScaleDimensions = new SizeF(7F, 15F);
    AutoScaleMode = AutoScaleMode.Font;

    ClientSize = new Size(500, 500);

    Controls.Add(plusButton);
    Controls.Add(minusButton);
    Controls.Add(scorePanel);
    Controls.Add(menuStrip);

    MainMenuStrip = menuStrip;
    Name = "Clicker";
    StartPosition = FormStartPosition.CenterScreen;
    Text = "Vuln Clicker";

    menuStrip.ResumeLayout(false);
    menuStrip.PerformLayout();

    scorePanel.ResumeLayout(false);
    ResumeLayout(false);
    PerformLayout();
  }

    private void RandomizeTargetLabelPosition()
    {
        // 50%の確率で位置を交換
        if (random.Next(2) == 0)
        {
            Point temp = plusTargetLabel.Location;
            plusTargetLabel.Location = minusTargetLabel.Location;
            minusTargetLabel.Location = temp;
        }
    }  
}