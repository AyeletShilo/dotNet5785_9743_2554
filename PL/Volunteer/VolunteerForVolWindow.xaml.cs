using PL.Call;
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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerForVolWindow.xaml
    /// </summary>
    public partial class VolunteerForVolWindow : Window
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static int _id;

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
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerForVolWindow), new PropertyMetadata(null));


        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(VolunteerForVolWindow), new PropertyMetadata(null));

        public BO.Role RoleType { get; set; } = BO.Role.Volunteer;



        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">id of who open this window</param>
        public VolunteerForVolWindow(int id = 0)
        {
            CurrentVolunteer = s_bl.Volunteer.Read(id);
            InitializeComponent();
            _id = id;
            if (CurrentVolunteer!.InCall != null)
            {
                CurrentCall = s_bl.Call.Read(CurrentVolunteer.InCall.CallId);
            }
        }

        private void RefreshVolunteer()
        {
            int id = CurrentVolunteer!.Id;
            CurrentVolunteer = null;
            CurrentVolunteer = s_bl.Volunteer.Read(id);
        }

        private void VolunteerObserver() => RefreshVolunteer();

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(VolunteerObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(VolunteerObserver);
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void UpdateVol_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerDataWindow(_id).Show();
        }

        private void ChoseCall_Click(object sender, RoutedEventArgs e)
        {
            new ChooseCallWindow(_id).Show();
        }

        private void HistoryCalls_Click(object sender, RoutedEventArgs e)
        {
            new HistoryCallsWindow(_id).Show();
        }

        private void EndTreatment_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentCall != null)
            {
                try
                {
                    s_bl.Call.UpdateEndTreatment(_id, CurrentCall.Id);
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void CancelTreatment_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentCall != null)
            {
                try
                {
                    s_bl.Call.UpdateCancelTreatment(CurrentVolunteer.Id, CurrentCall.Id);
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}




//private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
//{
//    try
//    {
//        if (ButtonText == "Add")
//        {
//            s_bl.Volunteer.Create(CurrentVolunteer!);
//            //MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
//        }
//        else if (ButtonText == "Update")
//        {
//            s_bl.Volunteer.Update(_id!, CurrentVolunteer!);
//            //MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
//        }
//        this.Close();
//    }
//    catch (Exception ex)
//    {
//        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//    }
//}
///// <summary>
///// DependencyProperty
///// </summary>
//public static readonly DependencyProperty ButtonTextProperty =
//    DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow), new PropertyMetadata(string.Empty));
//public string ButtonText
//{
//    get { return (string)GetValue(ButtonTextProperty); }
//    set { SetValue(ButtonTextProperty, value); }
//}