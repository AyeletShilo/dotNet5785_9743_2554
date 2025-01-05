using System.Windows;


namespace PL
{
    /// <summary>
    /// Interaction logic for ChoseWindow.xaml
    /// </summary>
    public partial class ChoseWindow : Window
    {
        static int _id;
        private static bool _isOpen = false;
        public ChoseWindow(int id)
        {
            InitializeComponent();
            _id = id;
        }
        private void OpenAdminWindow()
        {
            var adminWindow = new AdminWindow();
            _isOpen = true;
            adminWindow.Closed += (s, e) => _isOpen = false;
            adminWindow.ShowDialog();
        }

        private void Button_AdminClick(object sender, RoutedEventArgs e)
        {
            if (!_isOpen)
            { OpenAdminWindow(); }
            else
                MessageBox.Show("There is another administrator on the system, so the admin screen is unavailable. Please try again later :)",
                    "Please note", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
