using PL.Volunteer;
using System.Collections.ObjectModel;
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
        static private BO.Role user;
        private int _id;

        public string? TextInput
        {
            get { return (string)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("TextInput", typeof(string), typeof(MainWindow), new PropertyMetadata(null));
        private void btn_enter(object sender, RoutedEventArgs e)
        {
            try
            {

                if (int.TryParse(TextInput, out _id))
                {
                    user = s_bl.Volunteer.GetMyJob(_id);
                    if (user == BO.Role.Manager)
                    {
                        new ChoseWindow(_id).Show();
                        Close();
                    }
                    else if (user == BO.Role.Volunteer)
                    {
                        new VolunteerForVolWindow(_id).Show();
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Please enter correct ID again:)", "Exception",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text.Length == 9)
            {
                TextInput=textBox.Text;
            }
                //try
                //{
                //    //var textBox = sender as TextBox;
                //    _id = int.Parse(textBox.Text);
                //    user = s_bl.Volunteer.GetMyJob(_id);
                //    if (user == BO.Role.Manager)
                //    {
                //        new ChoseWindow(_id).Show();
                //        TextInput = null;
                //        //Close();
                //    }
                //    else if (user == BO.Role.Volunteer)
                //    {
                //        new VolunteerForVolWindow(_id).Show();
                //        Close();
                //    }

                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message + " Please enter correct ID again:)", "Exception",
                //        MessageBoxButton.OK, MessageBoxImage.Error);
                //}
        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                try
                {
                    _id = int.Parse(TextInput);
                    if (_id != null)
                    {
                        user = s_bl.Volunteer.GetMyJob(_id);
                        if (user == BO.Role.Manager)
                        {
                            new ChoseWindow(_id).Show();
                            Close();
                        }
                        else if (user == BO.Role.Volunteer)
                        {
                            new VolunteerForVolWindow(_id).Show();
                            Close();
                        }
                    }
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
    }
}