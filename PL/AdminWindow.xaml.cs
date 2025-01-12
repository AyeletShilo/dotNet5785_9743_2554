using BO;
using PL.Call;
using PL.Volunteer;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            queryCallList();
        }


        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public DateTime CurrentDate
        {
            get { return (DateTime)GetValue(CurrentDateProperty); }
            set { SetValue(CurrentDateProperty, value); }
        }
        public static readonly DependencyProperty CurrentDateProperty =
            DependencyProperty.Register("CurrentDate", typeof(DateTime), typeof(AdminWindow));

        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Minute);
        }
        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Hour);
        }
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Day);
        }
        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Month);
        }
        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Year);
        }
        public TimeSpan RiskRange
        {
            get { return (TimeSpan)GetValue(RiskRangeProperty); }
            set { SetValue(RiskRangeProperty, value); }
        }
        public static readonly DependencyProperty RiskRangeProperty =
            DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(AdminWindow));
        private void btnUpdateRiskRange_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetMaxRange(RiskRange);
        }
        private void clockObserver()
        {
            CurrentDate = s_bl.Admin.GetClock();
        }

        private void configObserver()
        {
            RiskRange = s_bl.Admin.GetMaxRange();
        }

        private void ScreenLoaded(object sender, RoutedEventArgs e) //?
        {
            CurrentDate = s_bl.Admin.GetClock();
            RiskRange = s_bl.Admin.GetMaxRange();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
            s_bl.Call.AddObserver(CallAmountObserver);
        }
        private void Window_Closed(object sender, /*Routed*/EventArgs e) //?
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
            s_bl.Call.RemoveObserver(CallAmountObserver);
        }

        private void btnVolunteer_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerListWindow().Show();
        }

        private void btnResetDB_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Do you want to do reset?", "Click to confirm:", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
            CloseAllWindowsExceptMain();
            Mouse.OverrideCursor = System.Windows.Input.Cursors.AppStarting;
            try
            {
                s_bl.Admin.ResetDB();
            }
            catch (Exception ex)
            {
                MessageBox.Show("The data was not reset properly. \nPlease Try Again:)",
                               "Exception",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
            Mouse.OverrideCursor = null;
        }
        private void btnInitializeDB_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Do you want to do an initialization?", "Click to confirm:", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
            CloseAllWindowsExceptMain();
            Mouse.OverrideCursor = System.Windows.Input.Cursors.AppStarting;
            try
            {
                s_bl.Admin.InitializeDB();
            }
            catch (Exception ex)
            {
                MessageBox.Show("The data was not initialized properly. \nPlease Try Again:)",
                               "Exception",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
            Mouse.OverrideCursor = null;
        }

        private void CloseAllWindowsExceptMain()
        {
            foreach (Window window in Application.Current.Windows.Cast<Window>().ToList())
                if (window != this)
                    window.Close();
        }

        private void btnCall_Click(object sender, RoutedEventArgs e)
        {
            new CallListWindow().Show();
        }



        public IEnumerable<int> CallsAmount
        {
            get { return (IEnumerable<int>)GetValue(CallsAmountProperty); }
            set { SetValue(CallsAmountProperty, value); }
        }

        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty CallsAmountProperty =
            DependencyProperty.Register("CallsAmount", typeof(IEnumerable<int>) ,typeof(AdminWindow));


        //public BO.CallListStatus Status { get; set; } = BO.CallListStatus.None;

        //private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    Status = (BO.CallListStatus)((ComboBox)sender).SelectedItem;
        //    queryCallList();
        //}

        private void queryCallList()
        {
            CallsAmount = s_bl.Call.HowManyCalls();
            //int Amount = CallsAmount[(int)Status];
        }
        private void CallAmountObserver()
            => queryCallList();

        private void Opened_Click(object sender, RoutedEventArgs e)
        {
            new CallListWindow(0, BO.CallListStatus.Opened).Show();
        }

        private void Closed_Click(object sender, RoutedEventArgs e)
        {
            new CallListWindow(0, BO.CallListStatus.Closed).Show();
        }
        private void Tretment_Click(object sender, RoutedEventArgs e)
        {
            new CallListWindow(0, BO.CallListStatus.InTreatment).Show();
        }
        private void Expired_Click(object sender, RoutedEventArgs e)
        {
            new CallListWindow(0, BO.CallListStatus.Expired).Show();
        }
        private void OpenRisk_Click(object sender, RoutedEventArgs e)
        {
            new CallListWindow(0, BO.CallListStatus.OpenInRisk).Show();
        }
        private void TretmentRisk_Click(object sender, RoutedEventArgs e)
        {
            new CallListWindow(0, BO.CallListStatus.InTreatmentInRisk).Show();
        }
    }
    
}