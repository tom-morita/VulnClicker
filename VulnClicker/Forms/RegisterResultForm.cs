namespace VulnClicker.Forms;

public partial class RegisterResultForm : Form
{
    public string PlayerName => playerNameTextBox.Text.Trim();

    public RegisterResultForm(
        string playerName,
        int score,
        int hit,
        int miss,
        int combo,
        int remainingTime)
    {
        InitializeComponent();

        remainingTimeValueLabel.Text = remainingTime.ToString();
        scoreValueLabel.Text = score.ToString();
        hitValueLabel.Text = hit.ToString();
        missValueLabel.Text = miss.ToString();
        comboValueLabel.Text = combo.ToString();

        playerNameTextBox.Text = playerName;
        playerNameTextBox.SelectAll();
        playerNameTextBox.Focus();
    }

    private void yesButton_Click(object? sender, EventArgs e)
    {
        string playerName = playerNameTextBox.Text.Trim();
        //playerName = resultForm.PlayerName;

        if (string.IsNullOrWhiteSpace(playerName))
        {
            MessageBox.Show(
                "プレイヤー名を入力してください。",
                "Vuln Clicker",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            playerNameTextBox.Focus();
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void noButton_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
