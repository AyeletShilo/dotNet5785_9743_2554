using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static int _id;

        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        public BO.Role RoleType { get; set; } = BO.Role.Volunteer;

        public ObservableCollection<BO.CallInProgress> InCall { get; set; }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow), new PropertyMetadata(string.Empty));
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    s_bl.Volunteer.Create(CurrentVolunteer!);
                    //MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (ButtonText == "Update")
                {
                    s_bl.Volunteer.Update(_id!, CurrentVolunteer!); 
                    //MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                this.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public VolunteerWindow(int id = 0)
        {
            ButtonText = id == 0 ? "Add" : "Update";
            _id = id;
            InitializeComponent();
            CurrentVolunteer = (id != 0) ? s_bl.Volunteer.Read(id)!
                : new BO.Volunteer()
                { Id = 0, FullName = "", PhoneNumber = "", Email = ""/*,Address="",*/ };
            //s_bl.AddObserver(id, VolunteerObserver);

            //this.Closed += Window_Closed;
            DataContext = this;

        }

        private void RefreshVolunteer()
        {
            int id = CurrentVolunteer!.Id; // שומר את ה-ID של הסטודנט הנוכחי
            CurrentVolunteer = null; // מנקה את הערך של CurrentStudent, כדי שה-Binding יתעדכן
            CurrentVolunteer = s_bl.Volunteer.Read(id); // טוען מחדש את הסטודנט
        }

        private void VolunteerObserver(/*BO.Volunteer updatedVolunteer*/) => RefreshVolunteer();
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        // מילוי מחדש של הפריט
        //        CurrentVolunteer = updatedVolunteer;
        //    });
        //}
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(VolunteerObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(VolunteerObserver);
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {

        }
    }
}
