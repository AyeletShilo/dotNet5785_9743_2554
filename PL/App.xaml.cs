using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace PL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        

        private Window _previousWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            EventManager.RegisterClassHandler(typeof(Button), Button.ClickEvent, new RoutedEventHandler(BackButton_Click));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Content.ToString() == "Back")
            {
                Window currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
                if (currentWindow != null && _previousWindow != null)
                {
                    _previousWindow.Show();
                    currentWindow.Close();
                }
            }
        }

        public void SetPreviousWindow(Window window)
        {
            _previousWindow = window;
        }
    }
}


