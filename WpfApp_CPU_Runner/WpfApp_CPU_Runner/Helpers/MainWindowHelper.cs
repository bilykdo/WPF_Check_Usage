using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp_CPU_Runner.Interfaces;

namespace WpfApp_CPU_Runner.Helpers
{
    public class MainWindowHelper : IMainWindowHelper
    {
        //public void StartCheck(string checkString, StackPanel stackPanel)
        //{
        //    var psReturn = this.GetPsObjects(checkString);

        //    foreach (var psObj in psReturn)
        //    {
        //        var id = psObj.Properties["ID"].Value;
        //        var processName = psObj.Properties["ProcessName"].Value;
        //        var workingSet = psObj.Properties[checkString].Value;

        //        Button button = new Button();
        //        button.Content = "Stop " + processName.ToString();
        //        button.Tag = id;
        //        button.Margin = new Thickness() { Left = 20 };
        //        button.Padding = new Thickness(3);
        //        button.AddHandler(Button.ClickEvent, new RoutedEventHandler(StopProcess));
        //        button.Background = Brushes.Yellow;

        //        var text = checkString + ": " + workingSet;
        //        var label = new Label()
        //        {
        //            Content = text,
        //            Padding = new Thickness() { Top = 10 },
        //            FontWeight = FontWeights.Bold
        //        };

        //        var textBlock = new TextBlock { Inlines = { label, button } };
        //        textBlock.Padding = new Thickness(5);
        //        textBlock.Tag = id;

        //        stackPanel.Children.Add(textBlock);
        //    }
        //}
        //public Collection<PSObject> GetPsObjects(string checkString)
        //{
        //    using var psCon = PowerShell.Create();
        //    string path =
        //        string.Format(
        //            @"Get-Process | Sort-Object -Descending {0} | Select –property ID, ProcessName, {0} | Select-Object -first 10",
        //            checkString);

        //    psCon.AddScript(path);

        //    var psReturn = psCon.Invoke();

        //    return psReturn;
        //}
        
        public static MainWindowHelper Instance { get; } = new MainWindowHelper();
    }
}
