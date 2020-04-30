using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfAnimatedGif;
using WpfApp_CPU_Runner.Helpers;
using WpfApp_CPU_Runner.Interfaces;

namespace WpfApp_CPU_Runner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public readonly IMainWindowHelper mainWindowHelper;
        private readonly DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();
            this.mainWindowHelper = MainWindowHelper.Instance;

            //Create a timer with interval of 4 secs
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 4);
        }

        private void StartCpuCheck(object sender, RoutedEventArgs e)
        {
            StackPanel.Children.Clear();
            this.StartCheck("CPU", StackPanel);
        }

        private void StartMemoryCheck(object sender, RoutedEventArgs e)
        {
            StackPanel.Children.Clear();
            this.StartCheck("WS", StackPanel);
        }

        void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            Gif.Visibility = Visibility.Hidden;
            (sender as DispatcherTimer)?.Stop();
        }

        public void StartCheck(string checkString, StackPanel stackPanel)
        {
            var psReturn = this.GetPsObjects(checkString);

            foreach (var psObj in psReturn)
            {
                var id = psObj.Properties["ID"].Value;
                var processName = psObj.Properties["ProcessName"].Value;
                var workingSet = psObj.Properties[checkString].Value;

                Button button = new Button();
                button.Content = "Stop " + processName.ToString();
                button.Tag = id;
                button.Margin = new Thickness() { Left = 20 };
                button.Padding = new Thickness(3);
                button.AddHandler(Button.ClickEvent, new RoutedEventHandler(StopProcess));
                button.Background = Brushes.Yellow;

                var text = checkString + ": " + workingSet;
                var label = new Label()
                {
                    Content = text,
                    Padding = new Thickness() { Top = 10 },
                    FontWeight = FontWeights.Bold
                };

                var textBlock = new TextBlock { Inlines = { label, button } };
                textBlock.Padding = new Thickness(5);
                textBlock.Tag = id;

                stackPanel.Children.Add(textBlock);
            }
        }
        public Collection<PSObject> GetPsObjects(string checkString)
        {
            using var psCon = PowerShell.Create();
            string path =
                string.Format(
                    @"Get-Process | Sort-Object -Descending {0} | Select –property ID, ProcessName, {0} | Select-Object -first 10",
                    checkString);

            psCon.AddScript(path);

            var psReturn = psCon.Invoke();

            return psReturn;
        }

        public void StopProcess(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var processId = button.Tag;

            // TODO: Extract this method so we can mock it
            Process process = Process.GetProcessById((int)processId);

            if (process.HasExited)
            {
                MessageBox.Show("This process is not running!", "CheckUsage App");
                return;
            }

            MessageBoxResult result = MessageBox.Show("Would you like to stop process: " + process.ProcessName + "? ", "CheckUsage App", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    SetImageBehaviorMethod("yes");

                    process.Kill();
                    button.Content = "Removed";
                    button.Background = Brushes.Green;

                    break;
                case MessageBoxResult.No:
                    SetImageBehaviorMethod("no");

                    //Start the timer
                    dispatcherTimer.Start();
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        private void SetImageBehaviorMethod(string imageName)
        {
            Gif.Visibility = Visibility.Visible;

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri($"Images/{imageName}.gif", UriKind.Relative);
            image.EndInit();
            ImageBehavior.SetAnimatedSource(Gif, image);

            //Start the timer
            dispatcherTimer.Start();
        }
    }
}
