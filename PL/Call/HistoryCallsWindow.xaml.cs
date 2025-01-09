using BO;
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
    /// Interaction logic for HistoryCallsWindow.xaml
    /// </summary>
    public partial class HistoryCallsWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private static int _id;

        public IEnumerable<BO.ClosedCallInList> CloseCallList
        {
            get { return (IEnumerable<BO.ClosedCallInList>)GetValue(CloseCallListProperty); }
            set { SetValue(CloseCallListProperty, value); }
        }
        public static readonly DependencyProperty CloseCallListProperty =
           DependencyProperty.Register("CloseCallList", typeof(IEnumerable<BO.ClosedCallInList>), typeof(HistoryCallsWindow), new PropertyMetadata(null));

        public BO.CallType CallFilter { get; set; } = BO.CallType.None;

        private void ComboBoxFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CallFilter = (BO.CallType)((ComboBox)sender).SelectedItem;
            queryCallList();
        }

        public BO.CloseCallData CallSort { get; set; } = BO.CloseCallData.Id;

        private void ComboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CallSort = (BO.CloseCallData)((ComboBox)sender).SelectedItem;
            queryCallList();
        }
        private void queryCallList()
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

        public BO.ClosedCallInList? SelectedCall { get; set; }
        public HistoryCallsWindow(int id)
        {
            _id = id;
            InitializeComponent();
        }
    }
}
