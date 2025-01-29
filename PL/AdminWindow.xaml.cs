using BlApi;
using BO;
using PL;
using PL.Call;
using PL.Volunteer;
using System.Security.Cryptography;
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
using System.Windows.Threading;

namespace PL
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private Window _preWind;
        private int adminId;

        /// <summary>
        /// Constructor
        /// </summary>
        public AdminWindow(Window preWind ,int id)
        {
            adminId = id;
            ButtonText = "Start Simulator";
            Interval = 1440;
            InitializeComponent();
            queryCallList();
            _preWind = preWind;
        }


        #region DependencyObjects
        public DateTime CurrentDate
        {
            get { return (DateTime)GetValue(CurrentDateProperty); }
            set { SetValue(CurrentDateProperty, value); }
        }
        public static readonly DependencyProperty CurrentDateProperty =
            DependencyProperty.Register("CurrentDate", typeof(DateTime), typeof(AdminWindow));

        public TimeSpan RiskRange
        {
            get { return (TimeSpan)GetValue(RiskRangeProperty); }
            set { SetValue(RiskRangeProperty, value); }
        }
        public static readonly DependencyProperty RiskRangeProperty =
            DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(AdminWindow));

        public IEnumerable<int> CallsAmount
        {
            get { return (IEnumerable<int>)GetValue(CallsAmountProperty); }
            set { SetValue(CallsAmountProperty, value); }
        }

        public static readonly DependencyProperty CallsAmountProperty =
            DependencyProperty.Register("CallsAmount", typeof(IEnumerable<int>), typeof(AdminWindow));

        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }

            set { SetValue(IntervalProperty, value); }
        }
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(AdminWindow));

        public bool SimulatorRun
        {
            get { return (bool)GetValue(SimulatorRunProperty); }

            set { SetValue(SimulatorRunProperty, value); }
        }
        public static readonly DependencyProperty SimulatorRunProperty =
            DependencyProperty.Register("SimulatorRun", typeof(bool), typeof(AdminWindow));

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow), new PropertyMetadata(string.Empty));
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }
        #endregion 


        #region Config area
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
        private void btnUpdateRiskRange_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetMaxRange(RiskRange);
        }

        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        private void clockObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    CurrentDate = s_bl.Admin.GetClock();
                });

            
        }

        private volatile DispatcherOperation? _observerOperation2 = null; //stage 7

        private void configObserver()
        {
            if (_observerOperation2 is null || _observerOperation2.Status == DispatcherOperationStatus.Completed)
                _observerOperation2 = Dispatcher.BeginInvoke(() =>
                {
                    RiskRange = s_bl.Admin.GetMaxRange();
                });
        }
        #endregion

        private void btnSimulator_Click(object sender, RoutedEventArgs e)
        {
            if (SimulatorRun == false)
            {
                s_bl.Admin.StartSimulator(Interval); //stage 7
                SimulatorRun = true;
                ButtonText = "Stop Simulator";
            }
            else
            {
                s_bl.Admin.StopSimulator(); //stage 7
                SimulatorRun = false;
                ButtonText = "Start Simulator";
            }

        }
        /// <summary>
        /// Screen loaded events
        /// </summary>
        private void ScreenLoaded(object sender, RoutedEventArgs e) 
        {
            CurrentDate = s_bl.Admin.GetClock();
            RiskRange = s_bl.Admin.GetMaxRange();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
            s_bl.Call.AddObserver(CallAmountObserver);
        }

        /// <summary>
        /// Screen closed events
        /// </summary>
        private void Window_Closed(object sender, /*Routed*/EventArgs e)
        {
            s_bl.Admin.StopSimulator();
            SimulatorRun= false;
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
            s_bl.Call.RemoveObserver(CallAmountObserver);
        }

        /// <summary>
        /// Open the volunteer list window
        /// </summary>
        private void btnVolunteer_Click(object sender, RoutedEventArgs e)
        {
            var nextWind = new VolunteerListWindow(this, adminId);
            nextWind.Show();
            //this.Hide();
            //new VolunteerListWindow().Show();
        }

        /// <summary>
        /// Open the call list window
        /// </summary>
        private void btnCall_Click(object sender, RoutedEventArgs e)
        {
            var nextWind = new CallListWindow(this, adminId);
            nextWind.Show();
           // this.Hide();
            //new CallListWindow().Show();
        }

        /// <summary>
        /// Reset data base
        /// </summary>
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

        /// <summary>
        /// Initialize data base
        /// </summary>
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

        
        #region Calls amounts
        private void Opened_Click(object sender, RoutedEventArgs e)
        {
            var nextWind = new CallListWindow(this, 0, BO.CallListStatus.Opened, false);
            nextWind.Show();
            //this.Hide();
            //new CallListWindow(this,0, BO.CallListStatus.Opened, false).Show();
        }

        private void Closed_Click(object sender, RoutedEventArgs e)
        {
            var nextWind = new CallListWindow(this, 0, BO.CallListStatus.Closed, false);
            nextWind.Show();
            //this.Hide();
           // new CallListWindow(this,0, BO.CallListStatus.Closed ,false).Show();
        }
        private void Treatment_Click(object sender, RoutedEventArgs e)
        {
            var nextWind = new CallListWindow(this, 0, BO.CallListStatus.InTreatment, false);
            nextWind.Show();
           // this.Hide();
            //new CallListWindow(this,0, BO.CallListStatus.InTreatment,false).Show();
        }
        private void Expired_Click(object sender, RoutedEventArgs e)
        {
            var nextWind = new CallListWindow(this, 0, BO.CallListStatus.Expired, false);
            nextWind.Show();
            //this.Hide();
            //new CallListWindow(this,0, BO.CallListStatus.Expired , false).Show();
        }
        private void OpenRisk_Click(object sender, RoutedEventArgs e)
        {
            var nextWind = new CallListWindow(this, 0, BO.CallListStatus.OpenInRisk, false);
            nextWind.Show();
            //this.Hide();
            //new CallListWindow(this,0, BO.CallListStatus.OpenInRisk , false).Show();
        }
        private void TreatmentRisk_Click(object sender, RoutedEventArgs e)
        {
            var nextWind = new CallListWindow(this, 0, BO.CallListStatus.InTreatmentInRisk, false);
            nextWind.Show();
           // this.Hide();
            //new CallListWindow(this,0, BO.CallListStatus.InTreatmentInRisk , false).Show();
        }

       
        private void queryCallList()
        {
            CallsAmount = s_bl.Call.HowManyCalls();
        }

        private volatile DispatcherOperation? _observerOperation3 = null; //stage 7
        private void CallAmountObserver()
        {
            if (_observerOperation3 is null || _observerOperation3.Status == DispatcherOperationStatus.Completed)
                _observerOperation3 = Dispatcher.BeginInvoke(() =>
                {

                    queryCallList();
                });   
        }
        #endregion
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.StopSimulator();
            SimulatorRun = false;
            _preWind.Show();
            this.Close();
        }
    }

   
}