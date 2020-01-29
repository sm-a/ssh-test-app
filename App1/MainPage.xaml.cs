using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        private SynchronizationContext sc = SynchronizationContext.Current;
        private bool IsInProgress { get; set; } = false; // Zur Verhinderung von mehrfachen Klicks
        private List<String> smaCommands;
        private String smaHost;
        private String smaUserName;
        private String smaPW;

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (IsInProgress) return; // wenn "In Arbeit", dann abbrechen
            this.IsInProgress = true; // "In Arbeit" setzen

            // Testen ob alle Felder ausgefüllt wurden
            if (smaHostTF.Text == "" || smaUserNameTF.Text == "" || smaPWTF.Text == "")
            {
                Debug.WriteLine("NICHT ALLE FELDER SIND AUSGEFÜLLT - WIRD DAHER ABGEBROCHEN!");
                //return;
            }

            smaCommands = new List<String>();

            smaCommands.Add("sleep 15");
            smaCommands.Add("sleep 10");
            smaCommands.Add("sleep 5");

            // progressbar initialisierung mit werten
            pbStatus.Maximum = smaCommands.Count;
            pbStatus.Value = 0;

            smaHost = smaHostTF.Text.Trim();
            smaUserName = smaUserNameTF.Text.Trim();
            smaPW = smaPWTF.Text.Trim();

            await Task.Run(new Action(DoWork));
        }



        private void DoWork()
        {
            ConnectionInfo conInfo = new ConnectionInfo(smaHost, 22, smaUserName, new AuthenticationMethod[]{
                    new PasswordAuthenticationMethod(smaUserName,smaPW)
            });

            // ssh client verbinden
            SshClient sshClient = new SshClient(conInfo);
            sshClient.Connect();

            foreach (var smaCommandsToProcess in smaCommands)
            {
                Debug.WriteLine("Aktuelles Kommando:\n" + smaCommandsToProcess);

                var command1 = sshClient.RunCommand(smaCommandsToProcess);
                var s1 = command1.Result;

                Debug.WriteLine("Aktuelles Kommando OUTPUT:\n" + smaCommandsToProcess + "\n" + s1);

                sc.Post(new SendOrPostCallback((state) => pbStatus.Value += 1), null);
            }

            sshClient.Disconnect();
            sshClient.Dispose();

            this.IsInProgress = false;
        }
    }
}