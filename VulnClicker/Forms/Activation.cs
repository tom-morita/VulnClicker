using System.Text;
using System.Security.Cryptography;
using VulnClicker.Models;

namespace VulnClicker.Forms;

public partial class Activation : Form
{
    // ==================================================
    // ハードコードされたシリアルコード
    // ==================================================

    private const string ValidSerialCode_Paid  = "VULN-CLICKER-2026";
    private const string ValidSerialHash_Debug = "27EE351C664A7467E0E8A2BFEB4DF67A9E05027B0502DD9D74343EBCA507F610";


    // ==================================================
    // 認証状態
    // ==================================================

    public static bool IsActivated { get; private set; }
    private readonly StartOptions startOptions;


    // ==================================================
    // コンストラクタ
    // ==================================================
    public Activation(StartOptions startOptions)
    {
        InitializeComponent();
        this.startOptions = startOptions;
    }


    // ==================================================
    // Activate
    // ==================================================

    private void activateButton_Click(
        object? sender,
        EventArgs e)
    {
        string serial = serialTextBox.Text.Trim().ToUpper();
        string serial_hash;

        byte[] bytes = Encoding.UTF8.GetBytes(serial);
        byte[] hash = SHA256.HashData(bytes);

        serial_hash = Convert.ToHexString(hash);


        if (serial == ValidSerialCode_Paid)
        {
            IsActivated = true;

            MessageBox.Show(
                "ライセンス認証に成功しました。 \r\n通常版 → 有料版",
                "Activation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            DialogResult = DialogResult.OK;
            Close();

            return;
        }
        else if (serial_hash == ValidSerialHash_Debug)
        {
            MessageBox.Show(
                "デバッグモードを有効にしました。",
                "DebugMode",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            startOptions.Profile = RuntimeProfile.Diagnostic;
            DialogResult = DialogResult.OK;
            Close();

            return;            
        }
        else
        {
            MessageBox.Show(
                "シリアルコードが正しくありません。\r\n  入力値:\r\n  "
                + serial + "\r\n  Hash:\r\n  " + serial_hash ,
                "Activation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );

            serialTextBox.SelectAll();
            serialTextBox.Focus();
        }
    }
}