using BO;
using DO;
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

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallListWindow.xaml
    /// </summary>
    public partial class CallListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public CallListWindow(int id = 0 , BO.CallListStatus status = BO.CallListStatus.None,bool isFilter = true)
        {
            IsFilter = isFilter;
            InitializeComponent();
            VolunteerId = id;
            CallFilter = status;
            queryCallList();
        }
        public int VolunteerId
        {
            get { return (int)GetValue(VolunteerIdProperty); }
            set { SetValue(VolunteerIdProperty, value); }
        }

        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty VolunteerIdProperty =
            DependencyProperty.Register("VolunteerId", typeof(int), typeof(CallListWindow), new PropertyMetadata(null));

        public IEnumerable<BO.CallInList> CallList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow), new PropertyMetadata(null));

        public BO.CallListStatus CallFilter { get; set; } = BO.CallListStatus.None;
        public BO.CallData CallSort { get; set; } = BO.CallData.CallId;
        public bool IsFilter { get; set; } = true;

        public BO.CallInList? SelectedCall { get; set; }
        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CallFilter = (BO.CallListStatus)((ComboBox)sender).SelectedItem;
            queryCallList();
        }

        private void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CallSort= (BO.CallData)((ComboBox)sender).SelectedItem;
            queryCallList();
        }
        private void queryCallList()
        {
            if (CallFilter == BO.CallListStatus.None && CallSort == BO.CallData.CallId)
                CallList = s_bl?.Call.ReadAll()!;
            else if(CallFilter == BO.CallListStatus.None)
                CallList = s_bl.Call.ReadAll(null, CallSort, null)!;
            else if(CallSort == BO.CallData.CallId)
                CallList = s_bl.Call.ReadAll(BO.CallData.Status, null, CallFilter)!;
            else
                CallList = s_bl.Call.ReadAll(BO.CallData.Status, CallSort, CallFilter)!;
        }
        private void callListObserver()
             => queryCallList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(callListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(callListObserver);

        private void lsvCallsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
                new CallWindow(SelectedCall.CallId).Show();
        }

        private void lsvCallList_AddCall(object sender, RoutedEventArgs e)
        {
            new AddCall().Show();
        }
        private void Delete_Call(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Do you sure you want to delete this call?", "Click to confirm:", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
            try
            {
                int id = SelectedCall!.CallId;
                s_bl.Call.Delete(id);
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show("call not exist");
            }
            catch (BO.BlCannotBeDeletedException ex)
            {
                MessageBox.Show("this call cannot be deleted");
            }
        }

        private void Cancel_Call(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Do you sure you want to cancel the assignment for this call?", "Click to confirm:", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
            try
            {
                int? assignmentId = SelectedCall!.Id;
                if (assignmentId is null)
                {
                    MessageBox.Show($"Assignment with ID: {assignmentId} cannot be canceled"); return;
                }
                int assId = (int)assignmentId;
                int volId = VolunteerId;
                s_bl.Call.UpdateCancelTreatment(volId, assId);
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(/*"call not exist"*/ "this call assignment cannot be canceled");
            }
            catch (BO.BlCannotBeDeletedException ex)
            {
                MessageBox.Show("this call assignment cannot be canceled");
            }

        }

    }
}

