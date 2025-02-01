using BO;
using PL.Call;
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
using System.Windows.Threading;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerDataWindow.xaml
    /// </summary>
    public partial class VolunteerDataWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private static int _id;
        private Window _preWind;

        /// <summary>
        /// constructor
        /// </summary>
        public VolunteerDataWindow(int id, Window preWind)
        {
            _id = id;
            InitializeComponent();
            CurrentVolunteer = s_bl.Volunteer.Read(_id);
            _preWind = preWind;
        }

        #region Property
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerDataWindow), new PropertyMetadata(null));

        public bool EnableUp
        {
            get { return (bool)GetValue(EnableUpProperty); }
            set { SetValue(EnableUpProperty, value); }
        }

        public static readonly DependencyProperty EnableUpProperty =
            DependencyProperty.Register("EnableUp", typeof(bool), typeof(VolunteerDataWindow), new PropertyMetadata(true));
        #endregion

        /// <summary>
        /// Reading volunteer details
        /// </summary>
        private void RefreshVolunteer()
        {
            int id = CurrentVolunteer!.Id;
            CurrentVolunteer = null;
            CurrentVolunteer = s_bl.Volunteer.Read(id);
        }

        /// <summary>
        /// Update volunteer details
        /// </summary>
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (EnableUp)
            {
                try
                {
                    s_bl.Volunteer.Update(_id!, CurrentVolunteer!);
                    _preWind.Show();
                    this.Close();
                }
                catch (BLTemporaryNotAvailableException)
                {
                    MessageBox.Show($"Cannot perform the operation since Simulator is running:)");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void AddressTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            EnableUp = false;

            if (sender is TextBox textBox)
            {
                string address = textBox.Text;

                if (string.IsNullOrEmpty(address))
                {
                   EnableUp= true;
                   CurrentVolunteer!.Latitude = null;
                   CurrentVolunteer!.Longitude = null;
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
                    CurrentVolunteer!.Latitude = coordinates[0]!.Value;
                    CurrentVolunteer!.Longitude = coordinates[1]!.Value;
                }
            }
        }


        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        private void VolunteerObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    RefreshVolunteer();
                });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
           => s_bl.Volunteer.AddObserver(VolunteerObserver);
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(VolunteerObserver);

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _preWind.Show();
            this.Close();
        }
    }
}
