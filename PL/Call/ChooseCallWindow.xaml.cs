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
    /// Interaction logic for ChooseCallWindow.xaml
    /// </summary>
    public partial class ChooseCallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private static int _id;

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

        private void ComboBoxFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CallFilter = (BO.CallType)((ComboBox)sender).SelectedItem;
            queryCallList();
        }

        public BO.OpenCallData CallSort { get; set; } = BO.OpenCallData.Id;

        private void ComboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CallSort = (BO.OpenCallData)((ComboBox)sender).SelectedItem;
            queryCallList();
        }
        private void queryCallList()
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

        public BO.OpenCallInList? SelectedCall { get; set; }

        public ChooseCallWindow(int id)
        {
            _id = id;
            InitializeComponent();
            Volunteer = s_bl.Volunteer.Read(id)!;
        }
        private void callListObserver()
            => queryCallList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(callListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(callListObserver);

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedCall != null)
                new CallDescriptionWindow(SelectedCall.Description).ShowDialog();
        }

        private void Choose_Call(object sender, EventArgs e)
        {
            if (SelectedCall != null)
                s_bl.Call.CallToTreatment(_id, SelectedCall.Id);
            queryCallList();
        }

        private void UpdateAdd_Click(object sender, RoutedEventArgs e)
        {
            //s_bl.Volunteer.UpdateAddress(Volunteer, Volunteer.Address);
            queryCallList();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
