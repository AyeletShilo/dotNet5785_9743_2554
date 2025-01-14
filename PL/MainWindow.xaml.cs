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
        static private BO.Role user;
        private int _id;

        public string? IDInput
        {
            get { return (string)GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }

        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty IDProperty =
            DependencyProperty.Register("IDInput", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

        public string PassInput
        {
            get { return (string)GetValue(PaaswordProperty); }
            set { SetValue(PaaswordProperty, value); }
        }

        public static readonly DependencyProperty PaaswordProperty =
            DependencyProperty.Register("PassInput", typeof(string), typeof(MainWindow), new PropertyMetadata(null));

        
        private void btn_enter(object sender, RoutedEventArgs e)
        {
            try
            {

                if (int.TryParse(IDInput, out _id))
                {
                    user = s_bl.Volunteer.GetMyJob(_id, PassInput);
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
        private void ID_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox.Text.Length == 9)
            {
                IDInput = textBox.Text;
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

        //private void Pass_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    var textBox = sender as TextBox;
        //    if (textBox.Text.Length == 8)
        //    {
        //        PassInput = textBox.Text;
        //    }
        //}

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PassInput = ((PasswordBox)sender).Password;
        }


        //private void TextBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //if (e.Key == Key.Enter)
        //{
        //    var app = (App)Application.Current;
        //    app.SetPreviousWindow(this);

        //    try
        //    {
        //        _id = int.Parse(IDInput);
        //        user = s_bl.Volunteer.GetMyJob(_id, PassInput);
        //        if (user == BO.Role.Manager)
        //        {
        //            new ChoseWindow(_id).Show();
        //            Close();
        //        }
        //        else if (user == BO.Role.Volunteer)
        //        {
        //            new VolunteerForVolWindow(_id).Show();
        //            Close();
        //        }

        //    }

        //    catch (BlPasswordException ex1)
        //    {
        //        MessageBox.Show(ex1.Message + "Please enter correct password again:)", "Exception",
        //            MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message + " Please enter correct ID again:)", "Exception",
        //            MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //}
        public MainWindow()
        {
            InitializeComponent();
        }

    }

}