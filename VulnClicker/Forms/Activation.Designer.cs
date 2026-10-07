namespace VulnClicker.Forms;

partial class Activation
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null!;

    private Label serialLabel;
    private TextBox serialTextBox;
    private Button activateButton;
    private Button cancelButton;


    // ==================================================
    // Dispose
    // ==================================================

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }


    #region Windows Form Designer generated code

    // ==================================================
    // InitializeComponent
    // ==================================================

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        
        serialLabel = new Label();
        serialTextBox = new TextBox();
        activateButton = new Button();
        cancelButton = new Button();

        SuspendLayout();


        // 
        // serialLabel
        // 
        serialLabel.AutoSize = false;
        serialLabel.Font = new Font(
            "Consolas",
            11F,
            FontStyle.Regular
        );
        serialLabel.Location = new Point(30, 25);
        serialLabel.Name = "serialLabel";
        serialLabel.Size = new Size(360, 25);
        serialLabel.TabIndex = 0;
        serialLabel.Text =
            "シリアルコードを入力してください";
        serialLabel.TextAlign =
            ContentAlignment.MiddleLeft;


        // 
        // serialTextBox
        // 
        serialTextBox.Font = new Font(
            "Consolas",
            11F,
            FontStyle.Regular
        );
        serialTextBox.Location = new Point(30, 60);
        serialTextBox.Name = "serialTextBox";
        serialTextBox.Size = new Size(360, 25);
        serialTextBox.TabIndex = 1;


        // 
        // activateButton
        // 
        activateButton.Font = new Font(
            "Consolas",
            10F,
            FontStyle.Regular
        );
        activateButton.Location =
            new Point(190, 110);
        activateButton.Name = "activateButton";
        activateButton.Size =
            new Size(95, 35);
        activateButton.TabIndex = 2;
        activateButton.Text = "Activate";
        activateButton.UseVisualStyleBackColor = true;

        activateButton.Click +=
            activateButton_Click;


        // 
        // cancelButton
        // 
        cancelButton.DialogResult =
            DialogResult.Cancel;

        cancelButton.Font = new Font(
            "Consolas",
            10F,
            FontStyle.Regular
        );
        cancelButton.Location =
            new Point(295, 110);
        cancelButton.Name = "cancelButton";
        cancelButton.Size =
            new Size(95, 35);
        cancelButton.TabIndex = 3;
        cancelButton.Text = "Cancel";
        cancelButton.UseVisualStyleBackColor = true;


        // 
        // Activation
        // 
        AcceptButton = activateButton;
        CancelButton = cancelButton;

        AutoScaleDimensions =
            new SizeF(7F, 15F);

        AutoScaleMode =
            AutoScaleMode.Font;

        ClientSize =
            new Size(420, 180);

        Controls.Add(serialLabel);
        Controls.Add(serialTextBox);
        Controls.Add(activateButton);
        Controls.Add(cancelButton);

        FormBorderStyle =
            FormBorderStyle.FixedDialog;

        MaximizeBox = false;
        MinimizeBox = false;

        Name = "Activation";

        StartPosition =
            FormStartPosition.CenterParent;

        Text = "Activation";

        ResumeLayout(false);
        PerformLayout();
    }

    #endregion
}