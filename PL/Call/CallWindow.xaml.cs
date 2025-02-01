using BO;
using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Threading;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private Window _preWind;
        

        //constructor
        public CallWindow(Window preWind, int id = 0)
        {
            InitializeComponent();
            CurrentCall = (id != 0) ? s_bl.Call.Read(id)!
                : new BO.Call() { Id = 0, CallAddress = "" };
            _preWind = preWind;
        }

        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));
       public bool EnableUp 
        {
            get { return (bool)GetValue(EnableUpProperty); }
            set { SetValue(EnableUpProperty, value); }
        }

        public static readonly DependencyProperty EnableUpProperty =
            DependencyProperty.Register("EnableUp", typeof(bool), typeof(CallWindow), new PropertyMetadata(true));

        /// <summary>
        /// Update call window
        /// </summary>
        public void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (EnableUp)
            {
                try
                {
                    if (CurrentCall!.CallAddress != "Wrong address" && CurrentCall.CallAddress != "Not In Range")
                    {
                        s_bl.Call.Update(CurrentCall!);
                        _preWind.Show(); //?
                        this.Close();
                    }
                }
                catch (BO.BlDoesNotExistException ex1)
                {
                    MessageBox.Show($"Call with ID={CurrentCall!.Id} does Not exists", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BO.BlXMLFileLoadCreateException ex2)
                {
                    MessageBox.Show($"Xml error", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BlIntegrityOfValuesException ex3)
                {
                    MessageBox.Show(ex3.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BLTemporaryNotAvailableException)
                {
                    MessageBox.Show($"Cannot perform the operation since Simulator is running:)");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "Please try again", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        private async void CallAddressTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            EnableUp = false;

            if (sender is TextBox textBox)
            {
                string address = textBox.Text;

                if (string.IsNullOrEmpty(address))
                {
                    MessageBox.Show("The address must contain a value.", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var coordinates = await s_bl.Call.CheckedAddress(address);

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
                    MessageBox.Show("Sorry, this is outside our scope of activity, but we'll be there soon :)", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    EnableUp = true;
                    CurrentCall!.Latitude = coordinates[0]!.Value;
                    CurrentCall!.Longitude = coordinates[1]!.Value;
                }
            }
        }


        /// <summary>
        /// Re-reading call's details
        /// </summary>
        private void RefreshCall()
        {
            int id = CurrentCall!.Id;
            CurrentCall = null;
            CurrentCall = s_bl.Call.Read(id);
        }

        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        private void CallObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {

                    RefreshCall();
                });
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(CallObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(CallObserver);

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _preWind.Show();
            this.Close();
        }
    }
}
