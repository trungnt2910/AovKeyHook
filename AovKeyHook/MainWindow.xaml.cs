using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AovKeyHook
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine = (o) =>
            {
                try
                {
                    lock (ConsoleView)
                    {
                        ConsoleView.MaxLines = 1000;
                        ConsoleView.AppendText($"{o}\n");
                        ConsoleView.CaretIndex = ConsoleView.Text.Length;
                        ConsoleView.ScrollToEnd();
                    }
                }
                catch
                {

                }
            };

            Hooker.HookMain(null);

            Closed += (s, a) => Hooker.UnhookMain();
        }

        private async void Run_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(CommandView.Text))
                {
                    return;
                }

                var text = CommandView.Text;
                Console.WriteLine(text);
                CommandView.Text = string.Empty;
                var parts = text.Split();
                var x = double.Parse(parts[0]);
                var y = double.Parse(parts[1]);
                await Hooker.ClickAsync(new Point(x, y));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
