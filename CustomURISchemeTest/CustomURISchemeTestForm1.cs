using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomURISchemeTest
{
    public partial class CustomURISchemeTestForm1 : Form
    {
        public CustomURISchemeTestForm1()
        {
            InitializeComponent();

            string[] args = System.Environment.GetCommandLineArgs();

            // サブコマンドがない場合は終了
            if (args.Length == 1)
            {
                Close();
            }

            string subCommand = args[1].ToLower();

            switch(subCommand)
            {
                case "register":
                    // レジストリ登録サブコマンド。引数adminをUAEありで起動する
                    RunAsAdministrator();
                    break;
                case "admin":
                    // 管理者権限で起動するサブコマンド。レジストリ登録用
                    RegisterCustomUriHandler();
                    Close();
                    break;
                case "process":
                    // カスタムURIスキーマを開いたときに呼び出されるサブコマンド。引数2つめにURLが格納される
                    ProcessCommandLineArgs();
                    break;
                default:
                    // 知らないサブコマンドの場合は終了
                    Close();
                    break;
            }

        }

        string GetExePath()
        {
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetEntryAssembly();
            string path = myAssembly.Location;
            return path;
        }

        void RunAsAdministrator()
        {

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.FileName = GetExePath();
            psi.Verb = "runas";
            psi.Arguments = "admin";

            if (this != null)
            {
                psi.ErrorDialog = true;
                psi.ErrorDialogParentHandle = this.Handle;
            }

            try
            {
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
                p.WaitForExit();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // cacel UAC....
                textBox1.Text = "UACがキャンセルされました...";
                return;

            }

            textBox1.Text = "レジストリ登録完了";
        }

        void RegisterCustomUriHandler()
        {
            string customSchemeString = "testtest1234scheme";

            // exeの絶対パスを取得
            string path = GetExePath();

            // カスタムURIハンドラの登録に必要な パラメータを登録する
            // exeのパス_process_カスタムURIスキーマのURL
            Microsoft.Win32.RegistryKey reg1 = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey($"{customSchemeString}");
            reg1.SetValue("URL Protocol", ""); // キーURL Protocolに空文字を追加する


            Microsoft.Win32.RegistryKey reg2 = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey($"{customSchemeString}\\shell\\open\\command");
            reg2.SetValue("", $"\"{path}\" process \"%1\"");  // 1番目の引数を空文字にして「(規定)」に起動時のコマンド・引数を指定
        }


        void ProcessCommandLineArgs()
        {
            string[] args = System.Environment.GetCommandLineArgs();
            if (args.Length >= 3)
            {
                textBox1.Text = args[2];
            }
            else
            {
                textBox1.Text = "process 引数なし (args.Length<3)";
            }

            textBox1.Select(textBox1.Text.Length, 0); // Textに代入すると選択状態になるので選択解除

        }
    }
}
