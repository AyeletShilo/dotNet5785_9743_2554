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
        }


        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public DateTime CurrentDate
        {
            get { return (DateTime)GetValue(CurrentDateProperty); }
            set { SetValue(CurrentDateProperty, value); }
        }
        public static readonly DependencyProperty CurrentDateProperty =
            DependencyProperty.Register("CurrentDate", typeof(DateTime), typeof(MainWindow));

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
        }
        private void Window_Closed(object sender, /*Routed*/EventArgs e) //?
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
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
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
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
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}



//using PL.Volunteer;
//using System.Collections.ObjectModel;
//using System.Numerics;
//using System.Windows;
//using System.Windows.Controls;

//namespace PL
//{
//    /// <summary>
//    /// Interaction logic for EntryWindow.xaml
//    /// </summary>
//    public partial class EntryWindow : Window
//    {
//        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
//        static private BO.Role user;
//        private int _id;
//        private static bool isAdminLoggedIn = false;
//        public bool IsButtonVEnabled { get; set; } = false;
//        public bool IsButtonMEnabled { get; set; } = false;

//        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
//        {
//            try
//            {
//                var textBox = sender as TextBox;
//                _id = int.Parse(textBox.Text);
//                user = s_bl.Volunteer.GetMyJob(_id);
//                if (user == BO.Role.Manager)
//                {
//                    IsButtonMEnabled = true;
//                }
//                IsButtonVEnabled = true;
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show( ex.Message + " Please enter correct ID again:)", "Exception",
//                    MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }
//        public EntryWindow()
//        {
//            InitializeComponent();
//        }

//        private void btn_OpenWindow(object sender, RoutedEventArgs e)
//        {
//            if (!isAdminLoggedIn)
//            {
//                isAdminLoggedIn = true;
//                MainWindow mainWindow = new MainWindow();
//                mainWindow.Show();
//                mainWindow.Closed += (s, args) => isAdminLoggedIn = false;
//            }
//            else
//            {
//                MessageBox.Show("An administrator is already logged in. Please try again later :)", "Attention",
//                    MessageBoxButton.OK, MessageBoxImage.Information);
//            }

//        }
//    }
//}

