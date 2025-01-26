using BO;
using PL.Volunteer;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }


        static private BO.Role user;
        private int _id;
        public string? Id { get; set; }
        public string Password { get; set; }


        /// <summary>
        /// Moving to the next window according to the volunteer's ID when the "Login" button is pressed
        /// </summary>
        private void btn_Login(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(Id, out _id))
                {
                    user = s_bl.Volunteer.GetMyJob(_id, Password);
                    if (user == BO.Role.Manager)
                    {
                        var nextWind = new ChoseWindow(this,_id);
                        nextWind.Show();
                        //this.Hide();

                        //new ChoseWindow(_id).Show();
                        //Close();
                    }
                    else if (user == BO.Role.Volunteer)
                    {
                        var nextWind = new VolunteerForVolWindow(this, _id);
                        nextWind.Show();
                        this.Hide();
                        //new VolunteerForVolWindow(this,_id).Show();
                        //Close();
                    }
                }
            }
            catch(BlPasswordException ex)
            {
                MessageBox.Show(ex.Message + " Please enter correct password again:)", "Exception",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Please enter correct ID again:)", "Exception",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Blocking inappropriate ID format
        /// </summary>
        private void ID_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text.Length == 9)
            {
                Id = textBox.Text;
            }
        }

        /// <summary>
        /// Converting the password from window display to string
        /// </summary>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = ((PasswordBox)sender).Password;
        }

        /// <summary>
        /// Moves the cursor to the password box when the volunteer presses Enter in the ID box.
        /// </summary>
        private void ID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // move to the next text box
                var textBox = sender as TextBox;
                textBox?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        /// <summary>
        /// Moving to the next window according to the volunteer's id and password when the volunteer presses Enter in password box
        /// </summary>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    _id = int.Parse(Id);
                    user = s_bl.Volunteer.GetMyJob(_id, Password);
                    if (user == BO.Role.Manager)
                    {
                        var nextWind = new ChoseWindow(this, _id);
                        nextWind.Show();
                        this.Hide();
                        //new ChoseWindow(_id).Show();
                        //Close();
                    }
                    else if (user == BO.Role.Volunteer)
                    {
                        var nextWind = new VolunteerForVolWindow(this, _id);
                        nextWind.Show();
                        this.Hide();
                        //new VolunteerForVolWindow(this,_id).Show();
                        //Close();
                    }
                }

                catch (BlPasswordException ex1)
                {
                    MessageBox.Show(ex1.Message + "Please enter correct password again:)", "Exception",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + " Please enter correct ID again:)", "Exception",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

    }

}