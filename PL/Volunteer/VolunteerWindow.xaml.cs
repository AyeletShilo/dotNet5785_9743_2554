using BO;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;


namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static int _id;
        private int _adminId;
        private Window _preWind;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Id of who open this window</param>
        public VolunteerWindow(Window preWind, int adminId,int id = 0)
        {
            UpdateText = id == 0 ? "Add" : "Update";
            InitializeComponent();
            _id = id;
            _adminId = adminId;
            CurrentVolunteer = (id != 0) ? s_bl.Volunteer.Read(id)!
                : new BO.Volunteer()
                { Id = 0, FullName = "", PhoneNumber = "", Email = "", Password = s_bl.Volunteer.MakeStrongPassword() };
            _preWind = preWind;
        }


        #region Dependency objects
        /// <summary>
        /// This volunteer
        /// </summary>
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        public BO.Role RoleType { get; set; } = BO.Role.Volunteer;

        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty UpdateTextProperty =
            DependencyProperty.Register("UpdateText", typeof(string), typeof(VolunteerWindow), new PropertyMetadata(string.Empty));
        public string UpdateText
        {
            get { return (string)GetValue(UpdateTextProperty); }
            set { SetValue(UpdateTextProperty, value); }
        }

        #endregion

        /// <summary>
        /// Volunteer add or update event when clicking a button
        /// </summary>
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UpdateText == "Add")
                {
                    s_bl.Volunteer.Create(CurrentVolunteer!);
                    //MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (UpdateText == "Update")
                {
                    s_bl.Volunteer.Update(_adminId, CurrentVolunteer!);
                    //MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                _preWind.Show();//?
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

        /// <summary>
        /// Returns the new volunteer after an update or addition.
        /// </summary>
        private void RefreshVolunteer()
        {
            int id = CurrentVolunteer!.Id; 
            CurrentVolunteer = null; 
            CurrentVolunteer = s_bl.Volunteer.Read(id); 
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
