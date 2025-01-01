using PL.Volunteer;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static private BO.Role user;
        private int _id;
        private static bool isAdminLoggedIn = false;
        public bool IsButtonVEnabled { get; set; } = false;
        public bool IsButtonMEnabled { get; set; } = false;

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var textBox = sender as TextBox;
                _id = int.Parse(textBox.Text);
                user = s_bl.Volunteer.GetMyJob(_id);
                if (user == BO.Role.Manager)
                {
                    IsButtonMEnabled = true;
                }
                IsButtonVEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Please enter correct ID again:)", "Exception",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_OpenWindow(object sender, RoutedEventArgs e)
        {
            if (!isAdminLoggedIn)
            {
                isAdminLoggedIn = true;
                AdminWindow adminWindow = new AdminWindow();
                adminWindow.Show();
                adminWindow.Closed += (s, args) => isAdminLoggedIn = false;
            }
            else
            {
                MessageBox.Show("An administrator is already logged in. Please try again later :)", "Attention",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
    }
}

