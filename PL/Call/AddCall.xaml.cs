using BO;
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
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for AddCall.xaml
    /// </summary>
    public partial class AddCall : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private Window _preWind;


        //constructor
        public AddCall(Window preWind)
        {
            InitializeComponent();
            CurrentCall = new BO.Call() { Id = 0};
            _preWind = preWind;
        }

        #region Properties

        public DateTime CurrentTime
        {
            get { return s_bl.Admin.GetClock(); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(AddCall), new PropertyMetadata(null));

        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(AddCall), new PropertyMetadata(null));

        public bool EnableUp
        {
            get { return (bool)GetValue(EnableUpProperty); }
            set { SetValue(EnableUpProperty, value); }
        }

        public static readonly DependencyProperty EnableUpProperty =
            DependencyProperty.Register("EnableUp", typeof(bool), typeof(AddCall), new PropertyMetadata(false));

        #endregion

        /// <summary>
        /// Adding call when clicking on a button
        /// </summary>
        public void bthAdd_Click(object sender, RoutedEventArgs e)
        {
            if (EnableUp)
            {
                try
                {
                    s_bl.Call.Create(CurrentCall!);
                    _preWind.Show();
                    this.Close();
                }
                catch (BlIntegrityOfValuesException ex3)
                {
                    MessageBox.Show(ex3.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BLTemporaryNotAvailableException ex4)
                {
                    MessageBox.Show($"Cannot perform the operation since Simulator is running:)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void CallAddressTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CurrentCall.CallAddress))
            {
                return;
            }
                if (sender is TextBox textBox)
            {
                string address = textBox.Text;

                var coordinates= await s_bl.Call.CheckedAddress(address);

                if (coordinates[0] == -1)
                {
                    MessageBox.Show("Network connection failed, please try again later :)", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (coordinates[0] == null)
                {
                    MessageBox.Show("Incorrect address, please enter a correct one.", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if ((coordinates![0] < 31.45 || coordinates[0] > 32) || (coordinates[1] < 34.85 || coordinates[1] > 35.4))
                {
                    MessageBox.Show("Sorry, this is outside our scope of activity, but we'll be there soon:) ", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    EnableUp = true;
                    CurrentCall.Latitude = coordinates[0]!.Value;
                    CurrentCall.Longitude = coordinates[1]!.Value;
                }
            }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _preWind.Show();
            this.Close();
        }
    }
}
    

