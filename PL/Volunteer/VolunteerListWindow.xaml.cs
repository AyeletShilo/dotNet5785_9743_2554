using BO;
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

        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        public BO.CallInTreatment CallType { get; set; } = BO.CallInTreatment.None;

        public BO.VolunteerData? SortValue { get; set; } = BO.VolunteerData.Id;
        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CallType = (BO.CallInTreatment)((ComboBox)sender).SelectedItem;

            queryVolunteerList();
        }

        private void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortValue = (BO.VolunteerData)((ComboBox)sender).SelectedItem;
            queryVolunteerList();
        }

        private void queryVolunteerList()
        {
            if (CallType == BO.CallInTreatment.None && SortValue== BO.VolunteerData.Id)
                VolunteerList = s_bl?.Volunteer.ReadAll()!;
            else if(CallType == BO.CallInTreatment.None)
                VolunteerList = s_bl?.Volunteer.ReadAll(null,SortValue)!;
            else if(SortValue == BO.VolunteerData.Id)
                VolunteerList = s_bl?.Volunteer.ReadAll(null, null, CallType)!;
            else
                VolunteerList= s_bl?.Volunteer.ReadAll(null, SortValue, CallType)!;
            
        }

        private void volunteerListObserver()
            => queryVolunteerList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(volunteerListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(volunteerListObserver);

        /// <summary>
        /// constructor
        /// </summary>
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
                MessageBox.Show("Volunteer not exist");
            }
            catch(BO.BlCannotBeDeletedException ex)
            {
                MessageBox.Show("this volunteer cannot be deleted");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
