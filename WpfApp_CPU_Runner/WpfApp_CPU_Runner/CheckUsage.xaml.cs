using System.Diagnostics;
using System.Management.Automation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp_CPU_Runner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartCPUCheck(object sender, RoutedEventArgs e)
        {
            StackPanel.Children.Clear();
            StartCheck("CPU");
        }

        private void StartMemoryCheck(object sender, RoutedEventArgs e)
        {
            StackPanel.Children.Clear();
            StartCheck("WS");
        }

        private void StartCheck(string checkString)
        {
            using (var psCon = PowerShell.Create())
            {
                string path = string.Format(@"Get-Process | Sort-Object -Descending {0} | Select –property ID, ProcessName, {0} | Select-Object -first 10", checkString);

                psCon.AddScript(path);

                var psReturn = psCon.Invoke();

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

                    StackPanel.Children.Add(textBlock);
                }
            }
        }

        void StopProcess(object sender, RoutedEventArgs e) {

            Button button = (Button)sender;
            var processId = button.Tag;

            Process process = Process.GetProcessById((int)processId);
            process.Kill();

            button.Content = "Removed";
            button.Background = Brushes.Green;

        }
    }
}
