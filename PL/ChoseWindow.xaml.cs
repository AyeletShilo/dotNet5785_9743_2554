using PL.Call;
using PL.Volunteer;
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
        private Window _preWind;
        public ChoseWindow(Window preWind ,int id)
        {
            InitializeComponent();
            _id = id;
            _preWind = preWind;
        }

        /// <summary>
        /// Open the volunteer management window
        /// </summary>
        private void OpenAdminWindow()
        {
           // new AdminWindow().Show();
            var adminWindow = new AdminWindow(this, _id);
            _isOpen = true;
            adminWindow.Closed += (s, e) => _isOpen = false;
            //var nextWind = new AdminWindow(this);
            adminWindow.Show();
            this.Hide();
        }

        /// <summary>
        /// Locking the volunteer management window to one administrator
        /// </summary>
        private void Button_AdminClick(object sender, RoutedEventArgs e)
        {
            if (!_isOpen)
            { 
                OpenAdminWindow(); 
            }
            else
                MessageBox.Show("There is another administrator on the system, so the admin screen is unavailable. Please try again later :)",
                    "Please note", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        ///Open the volunteer details window
        /// </summary>
        private void Volunteer_Click(object sender, RoutedEventArgs e)
        {
            var nextWind = new VolunteerForVolWindow(this,_id);
            nextWind.Show();
            this.Hide();

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _preWind.Show();
            this.Close();
        }
    }
}
