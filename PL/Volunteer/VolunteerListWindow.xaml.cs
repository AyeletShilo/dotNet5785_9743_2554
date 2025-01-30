using BO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;


namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private Window _preWind;
        private int _adminId;

        /// <summary>
        /// Constructor
        /// </summary>
        public VolunteerListWindow(Window preWind, int adminId)
        {
            InitializeComponent();
            _preWind = preWind;
            _adminId = adminId;
        }

        #region Dependency objects
        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        public BO.CallInTreatment CallType { get; set; } = BO.CallInTreatment.None;

        public BO.VolunteerData? SortValue { get; set; } = BO.VolunteerData.Id;
        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        #endregion

        /// <summary>
        /// Filter Volunteers by type of call in volunteer care
        /// </summary>
        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CallType = (BO.CallInTreatment)((ComboBox)sender).SelectedItem;

            queryVolunteerList();
        }

        /// <summary>
        /// Sort the volunteers by volunteer values
        /// </summary>
        private void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortValue = (BO.VolunteerData)((ComboBox)sender).SelectedItem;
            queryVolunteerList();
        }

        /// <summary>
        /// Refreshing the volunteer list after filtering or sorting
        /// </summary>
        private void queryVolunteerList()
        {
            try
            {
                if (CallType == BO.CallInTreatment.None && SortValue == BO.VolunteerData.Id)
                    VolunteerList = s_bl?.Volunteer.ReadAll()!;
                else if (CallType == BO.CallInTreatment.None)
                    VolunteerList = s_bl?.Volunteer.ReadAll(null, SortValue)!;
                else if (SortValue == BO.VolunteerData.Id)
                    VolunteerList = s_bl?.Volunteer.ReadAll(null, null, CallType)!;
                else
                    VolunteerList = s_bl?.Volunteer.ReadAll(null, SortValue, CallType)!;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Please try again", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        private void volunteerListObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryVolunteerList();
                });  
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(volunteerListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(volunteerListObserver);

        /// <summary>
        /// Opening a volunteer's details window when clicking on a volunteer in the list
        /// </summary>
        private void lsvVolunteersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
            {
                var nextWind = new VolunteerWindow(this,_adminId,SelectedVolunteer.Id);
                nextWind.Show();
               
            }
        }

        /// <summary>
        /// Open the Add Volunteer window
        /// </summary>
        private void lsvVolunteersList_AddVolunteer(object sender, RoutedEventArgs e)
        {
            var nextWind = new VolunteerWindow(this, _adminId);
            nextWind.Show();
        }

        /// <summary>
        /// Deleting a volunteer from the list
        /// </summary>
        private void Delete_Volunteer(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Do you sure you want to delete this volunteer?", "Click to confirm:", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
            try
            {
                int id = SelectedVolunteer!.Id;
                s_bl.Volunteer.Delete(id);
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show("Volunteer not exist", "Exception",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            catch(BO.BlCannotBeDeletedException ex)
            {
                MessageBox.Show("You can't delete this volunteer", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _preWind.Show();
            this.Close();
        }
    }
}
