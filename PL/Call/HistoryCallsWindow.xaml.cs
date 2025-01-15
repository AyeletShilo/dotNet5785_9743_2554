using System.Windows;
using System.Windows.Controls;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for HistoryCallsWindow.xaml
    /// </summary>
    public partial class HistoryCallsWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private static int _id;
        private Window _preWind;

        public HistoryCallsWindow(int id, Window preWind)
        {
            _id = id;
            InitializeComponent();
            _preWind = preWind;
        }

        #region propeties
        public IEnumerable<BO.ClosedCallInList> CloseCallList
        {
            get { return (IEnumerable<BO.ClosedCallInList>)GetValue(CloseCallListProperty); }
            set { SetValue(CloseCallListProperty, value); }
        }
        public static readonly DependencyProperty CloseCallListProperty =
           DependencyProperty.Register("CloseCallList", typeof(IEnumerable<BO.ClosedCallInList>), typeof(HistoryCallsWindow), new PropertyMetadata(null));

        public BO.CallType CallFilter { get; set; } = BO.CallType.None;
        public BO.CloseCallData CallSort { get; set; } = BO.CloseCallData.Id;
        public BO.ClosedCallInList? SelectedCall { get; set; }
        #endregion



        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CallFilter = (BO.CallType)((ComboBox)sender).SelectedItem;
            queryCallList();
        }

        private void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CallSort = (BO.CloseCallData)((ComboBox)sender).SelectedItem;
            queryCallList();
        }
        

        private void queryCallList()
        {
            try
            {
                if (CallSort == BO.CloseCallData.Id)
                {
                    if (CallFilter == BO.CallType.None)
                        CloseCallList = s_bl?.Call.GetClosedCalls(_id)!;
                    else
                        CloseCallList = s_bl?.Call.GetClosedCalls(_id, CallFilter)!;
                }
                else
                {
                    if (CallFilter == BO.CallType.None)
                        CloseCallList = s_bl?.Call.GetClosedCalls(_id, null, CallSort)!;
                    else
                        CloseCallList = s_bl?.Call.GetClosedCalls(_id, CallFilter, CallSort)!;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Please try again", "Exception",MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _preWind.Show();
            this.Close();
        }

    }
}
