using BO;
using DalApi;
using PL.Volunteer;
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

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for ChooseCallWindow.xaml
    /// </summary>
    public partial class ChooseCallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private static int _id;
        private VolunteerForVolWindow _preWind;

        //constructor
        public ChooseCallWindow(int id, VolunteerForVolWindow preWind)
        {
            _id = id;
            InitializeComponent();
            Volunteer = s_bl.Volunteer.Read(id)!;
            _preWind = preWind;
        }

        #region Propertis
        public IEnumerable<BO.OpenCallInList> OpenCallList
        {
            get { return (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallListProperty); }
            set { SetValue(OpenCallListProperty, value); }
        }
        public static readonly DependencyProperty OpenCallListProperty =
           DependencyProperty.Register("OpenCallList", typeof(IEnumerable<BO.OpenCallInList>), typeof(ChooseCallWindow), new PropertyMetadata(null));

        public BO.Volunteer Volunteer
        {
            get { return (BO.Volunteer)GetValue(VolunteerProperty); }
            set { SetValue(VolunteerProperty, value); }
        }
        public static readonly DependencyProperty VolunteerProperty =
           DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(ChooseCallWindow), new PropertyMetadata(null));

        public BO.CallType CallFilter { get; set; } = BO.CallType.None;
        public BO.OpenCallData CallSort { get; set; } = BO.OpenCallData.Id;
        public BO.OpenCallInList? SelectedCall { get; set; }

        #endregion

        /// <summary>
        /// Filter calls by type
        /// </summary>
        private void ComboBoxFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CallFilter = (BO.CallType)((ComboBox)sender).SelectedItem;
            queryCallList();
        }

        /// <summary>
        /// Sort calls by different values
        /// </summary>
        private void ComboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CallSort = (BO.OpenCallData)((ComboBox)sender).SelectedItem;
            queryCallList();
        }

        /// <summary>
        /// Re-reading the calls history after filtering or sorting the calls
        /// </summary>
        private void queryCallList()
        {
            try
            {
                if (CallSort == BO.OpenCallData.Id)
                {
                    if (CallFilter == BO.CallType.None)
                        OpenCallList = s_bl?.Call.GetOpenedCalls(_id)!;
                    else
                        OpenCallList = s_bl?.Call.GetOpenedCalls(_id, CallFilter)!;
                }
                else
                {
                    if (CallFilter == BO.CallType.None)
                        OpenCallList = s_bl?.Call.GetOpenedCalls(_id, null, CallSort)!;
                    else
                        OpenCallList = s_bl?.Call.GetOpenedCalls(_id, CallFilter, CallSort)!;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Please try again", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        private void callListObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryCallList();
                });
        }

        
        

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(callListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(callListObserver);

       
        /// <summary>
        /// Change of volunteer address
        /// </summary>
        private void Address_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Volunteer.Update(_id, Volunteer);
                Volunteer = s_bl.Volunteer.Read(_id)!;
                queryCallList();
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _preWind.RefreshVolunteer();
            _preWind.Show();
            this.Close();
        }

        /// <summary>
        /// Open call details when clicking a call in the list
        /// </summary>
        private void DataGrid_SelectionChanged(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
                new CallDescriptionWindow(SelectedCall.Description, SelectedCall.CallType).ShowDialog();
        }


        /// <summary>
        /// Choosing a call for treatment
        /// </summary>
        private void Choose_Call(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedCall != null)
                    s_bl.Call.CallToTreatment(_id, SelectedCall.Id);
                Close();
                //queryCallList();
                //_preWind.RefreshVolunteer();
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show($"Call with ID={SelectedCall!.Id} does not exists\"", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlDoesAlreadyExistException)
            {
                MessageBox.Show($"Assignment for call with ID={SelectedCall!.Id} already exists\"", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlCantHandleItException)
            {
                MessageBox.Show("Unable to assign");
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
}
