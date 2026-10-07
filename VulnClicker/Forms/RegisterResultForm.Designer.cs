using System;
using System.Windows.Forms;

namespace VulnClicker.Forms;


partial class RegisterResultForm
{
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.Label titleLabel;
    private System.Windows.Forms.Label playerNameLabel;
    private System.Windows.Forms.TextBox playerNameTextBox;
    private Label scoreLabel;
    private Label scoreValueLabel;

    private Label hitLabel;
    private Label hitValueLabel;

    private Label missLabel;
    private Label missValueLabel;

    private Label comboLabel;
    private Label comboValueLabel;
    private Label remainingTimeLabel;
    private Label remainingTimeValueLabel;
    private System.Windows.Forms.Button yesButton;
    private System.Windows.Forms.Button noButton;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        titleLabel =        new Label();

        playerNameLabel =   new Label();
        playerNameTextBox = new TextBox();

        scoreLabel =      new Label();
        scoreValueLabel = new Label();
        hitLabel =        new Label();
        hitValueLabel =   new Label();
        missLabel =       new Label();
        missValueLabel =  new Label();
        comboLabel =      new Label();
        comboValueLabel = new Label();
        remainingTimeLabel =      new Label();
        remainingTimeValueLabel = new Label();

        yesButton = new Button();
        noButton =  new Button();
        SuspendLayout();

        // 
        // titleLabel
        // 
        //titleLabel.AutoSize = true;
        titleLabel.Font = new System.Drawing.Font(
            "Consolas",
            14F,
            System.Drawing.FontStyle.Bold);

        titleLabel.Location =
            new System.Drawing.Point(55, 25);

        titleLabel.Name =
            "titleLabel";

        titleLabel.Size =
            new System.Drawing.Size(166, 25);

        titleLabel.TabIndex = 0;
        titleLabel.Text = "GAME OVER\nスコアを登録しますか?";

        // 
        // playerNameLabel
        // 
        //playerNameLabel.AutoSize = true;

        playerNameLabel.Font =
            new Font(
                "Consolas",
                10F);

        playerNameLabel.Location =
            new Point(40, 80);

        playerNameLabel.Name =
            "playerNameLabel";

        playerNameLabel.Size =
            new Size(100, 25);

        playerNameLabel.TabIndex = 1;

        playerNameLabel.Text =
            "プレイヤー名：";

        // 
        // playerNameTextBox
        // 
        playerNameTextBox.Font =
            new Font(
                "Consolas",
                11F);

        playerNameTextBox.Location =
            new Point(150, 77);

        playerNameTextBox.Name =
            "playerNameTextBox";

        playerNameTextBox.Size =
            new Size(200, 25);

        //playerNameTextBox.TabIndex = 2;


        scoreLabel.Text =          "Score:";
        scoreLabel.Location =      new Point(40, 130);
        scoreLabel.Size =          new Size(100, 25);
        scoreValueLabel.Location = new Point(150, 130);
        scoreValueLabel.Size =     new Size(200, 25);

        hitLabel.Text =            "Hit:";
        hitLabel.Location =        new Point(40, 160);
        hitLabel.Size =            new Size(100, 25);
        hitValueLabel.Location =   new Point(150, 160);
        hitValueLabel.Size =       new Size(200, 25);

        missLabel.Text =           "Miss:";
        missLabel.Location =       new Point(40, 190);
        missLabel.Size =           new Size(100, 25);
        missValueLabel.Location =  new Point(150, 190);
        missValueLabel.Size =      new Size(200, 25);

        comboLabel.Text =          "Combo:";
        comboLabel.Location =      new Point(40, 220);
        comboLabel.Size =          new Size(100, 25);
        comboValueLabel.Location = new Point(150, 220);
        comboValueLabel.Size =     new Size(200, 25);

        remainingTimeLabel.Text =          "Remaining Time:";
        remainingTimeLabel.Location =      new Point(40, 250);
        remainingTimeLabel.Size =          new Size(100, 25);
        remainingTimeValueLabel.Location = new Point(150, 250);
        remainingTimeValueLabel.Size =     new Size(200, 25);
        // 
        // yesButton
        // 
        yesButton.Font =
            new Font(
                "Consolas",
                10F);

        yesButton.Location = new Point(90, 310);
        yesButton.Name = "yesButton";
        yesButton.Size = new Size(100, 40);

        yesButton.Click +=
            new System.EventHandler(
                this.yesButton_Click);

        //this.yesButton.TabIndex = 3;

        yesButton.Text = "YES";
        yesButton.UseVisualStyleBackColor = true;

        // 
        // noButton
        // 
        noButton.Font =
            new Font(
                "Consolas",
                10F);
        noButton.Location = new Point(210, 310);
        noButton.Name = "noButton";
        noButton.Size = new Size(100, 40);

        //this.noButton.TabIndex = 3;

        noButton.Text = "NO";
        noButton.UseVisualStyleBackColor = true;
        noButton.Click +=
            new System.EventHandler(
                this.noButton_Click);

        // 
        // RegisterResultForm
        // 
        AcceptButton = yesButton;
        CancelButton = noButton;

        //AutoScaleDimensions = new SizeF(7F, 15F);
        //AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new Size(400, 400);

        Controls.Add(yesButton);
        Controls.Add(noButton);
        Controls.Add(playerNameTextBox);
        Controls.Add(playerNameLabel);
        Controls.Add(titleLabel);

        Controls.Add(scoreLabel);
        Controls.Add(scoreValueLabel);
        Controls.Add(hitLabel);
        Controls.Add(hitValueLabel);
        Controls.Add(missLabel);
        Controls.Add(missValueLabel);
        Controls.Add(comboLabel);
        Controls.Add(comboValueLabel);
        Controls.Add(remainingTimeLabel);
        Controls.Add(remainingTimeValueLabel);

        FormBorderStyle = FormBorderStyle.FixedDialog;

        MaximizeBox = false;
        MinimizeBox = false;

        Name = "RegisterResultForm";

        StartPosition = FormStartPosition.CenterParent;

        Text = "VulnClicker";

        ResumeLayout(false);
        PerformLayout();
    }
}
