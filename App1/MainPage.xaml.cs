using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Renci.SshNet;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // Testen ob alle Felder ausgefüllt wurden
            if (smaHostTF.Text == "" || smaUserNameTF.Text == "" || smaPWTF.Text == "")
            {
                Debug.WriteLine("NICHT ALLE FELDER SIND AUSGEFÜLLT - WIRD DAHER ABGEBROCHEN!");
                return;
            }

            String smaHost = smaHostTF.Text.Trim();
            String smaUserName = smaUserNameTF.Text.Trim();
            String smaPW = smaPWTF.Text.Trim();

            List<String> smaCommands = new List<String>();

            smaCommands.Add("sleep 15");
            smaCommands.Add("sleep 10");
            smaCommands.Add("sleep 5");

            ConnectionInfo conInfo = new ConnectionInfo(smaHost, 22, smaUserName, new AuthenticationMethod[]{
                    new PasswordAuthenticationMethod(smaUserName,smaPW)
            });

            // ssh client verbinden
            SshClient sshClient = new SshClient(conInfo);
            sshClient.Connect();

            // progressbar initialisierung mit werten
            pbStatus.Maximum = smaCommands.Count;
            pbStatus.Value = 0;

            foreach (var smaCommandsToProcess in smaCommands)
            {
                Debug.WriteLine("Aktuelles Kommando:\n" + smaCommandsToProcess);

                var command1 = sshClient.RunCommand(smaCommandsToProcess);
                var s1 = command1.Result;

                Debug.WriteLine("Aktuelles Kommando OUTPUT:\n" + smaCommandsToProcess + "\n" + s1);

                pbStatus.Value += 1;
            }

            sshClient.Disconnect();
            sshClient.Dispose();
        }
    }
}
