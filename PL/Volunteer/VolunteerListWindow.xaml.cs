using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        public BO.CallInTreatment CallType { get; set; } = BO.CallInTreatment.None;
        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CallType = (BO.CallInTreatment)((ComboBox)sender).SelectedItem;

            queryVolunteerList();
            //VolunteerList = (CallType == BO.CallInTreatment.None)
            //    ? s_bl?.Volunteer.ReadAll()!
            //    : s_bl?.Volunteer.ReadAll(null, BO.VolunteerData.InTreatment, CallType)!;
        }

        private void queryVolunteerList()
    => VolunteerList = (CallType == BO.CallInTreatment.None) ?
        s_bl?.Volunteer.ReadAll()! : s_bl?.Volunteer.ReadAll(null, BO.VolunteerData.InTreatment, CallType)!;

        private void volunteerListObserver()
            => queryVolunteerList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(volunteerListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(volunteerListObserver);

        public VolunteerListWindow()
        {
            InitializeComponent();
        }

        private void lsvVolunteersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
                new VolunteerWindow(SelectedVolunteer.Id).Show();
        }

        private void lsvVolunteersList_AddVolunteer(object sender, RoutedEventArgs e)
        {
                new VolunteerWindow().Show();
        }
        
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
