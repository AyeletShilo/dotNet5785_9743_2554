using PL.Call;
using System.Windows;
using System.Windows.Controls;


namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerForVolWindow.xaml
    /// </summary>
    public partial class VolunteerForVolWindow : Window
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static int _id;
        static string? _distance;
        private Window _preWind;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">id of who open this window</param>
        public VolunteerForVolWindow(Window preWind ,int id = 0 )
        {
            CurrentVolunteer = s_bl.Volunteer.Read(id);
            InitializeComponent();
            _id = id;
            _preWind = preWind;
            try
            {
                if (CurrentVolunteer!.InCall != null)
                {
                    CurrentCall = s_bl.Call.Read(CurrentVolunteer.InCall.CallId);
                    _distance = s_bl.Volunteer.Dis(CurrentVolunteer!.Address, CurrentCall.CallAddress).ToString();
                }
            }
          
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Please try again", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        
        }

        #region properties
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerForVolWindow), new PropertyMetadata(null));


        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(VolunteerForVolWindow), new PropertyMetadata(null));

        public char FirstLet => CurrentVolunteer!.FullName[0];
        //{
        //    get { return CurrentVolunteer!.FullName[0]; }
        //}
        public string? Distance => _distance;
        //{
        //    get { return _distance; }
        //}

        //public static readonly DependencyProperty DistanceProperty =
        //    DependencyProperty.Register("Distance", typeof(string), typeof(VolunteerForVolWindow), new PropertyMetadata(null));
        public BO.Role RoleType { get; set; } = BO.Role.Volunteer;

        #endregion


        private void RefreshVolunteer()
        {
            try
            {
                int id = CurrentVolunteer!.Id;
                CurrentVolunteer = null;
                CurrentVolunteer = s_bl.Volunteer.Read(id);
                if (CurrentVolunteer!.InCall != null)
                {
                    CurrentCall = s_bl.Call.Read(CurrentVolunteer.InCall.CallId);
                    _distance = s_bl.Volunteer.Dis(CurrentVolunteer!.Address, CurrentCall.CallAddress).ToString();
                }
                char refresh = FirstLet;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Please try again", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void VolunteerObserver() => RefreshVolunteer();

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(VolunteerObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(VolunteerObserver);
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        #region buttons
        private void UpdateVol_Click(object sender, RoutedEventArgs e)
        {
            var nextWind = new VolunteerDataWindow(_id, this);
            nextWind.Show();
            this.Hide();
            //new VolunteerDataWindow(_id).Show();

        }

        private void ChoseCall_Click(object sender, RoutedEventArgs e)
        {
            var nextWind = new ChooseCallWindow(_id, this);
            nextWind.Show();
            this.Hide();
            //new ChooseCallWindow(_id).Show();
        }

        private void HistoryCalls_Click(object sender, RoutedEventArgs e)
        {
            var nextWind = new HistoryCallsWindow(_id, this);
            nextWind.Show();
            this.Hide();
            //new HistoryCallsWindow(_id).Show();
        }

        private void EndTreatment_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentCall != null)
            {
                try
                {
                    s_bl.Call.GetAssignmentToEnd(_id, CurrentCall.Id);
                    RefreshVolunteer();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelTreatment_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentCall != null)
            {
                try
                {
                    s_bl.Call.GetAssignmentToCancel(CurrentVolunteer.Id, CurrentCall.Id);
                    RefreshVolunteer();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _preWind.Show();
            this.Close();
        }
    }
}