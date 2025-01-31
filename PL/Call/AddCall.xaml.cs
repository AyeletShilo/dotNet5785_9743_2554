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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for AddCall.xaml
    /// </summary>
    public partial class AddCall : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private Window _preWind;

        //constructor
        public AddCall(Window preWind)
        {
            InitializeComponent();
            CurrentCall = new BO.Call() { Id = 0};
            _preWind = preWind;
        }

        #region Properties

        //public DateTime CurrentTime
        //{
        //    get { return s_bl.Admin.GetClock(); }
        //}
        public DateTime CurrentTime
        {
            get { return s_bl.Admin.GetClock(); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(AddCall), new PropertyMetadata(null));

        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(AddCall), new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// Adding call when clicking on a button
        /// </summary>
        public void bthAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.Create(CurrentCall!);
                _preWind.Show();//?
                this.Close();
            }
            catch (BlIntegrityOfValuesException ex3)
            {
                MessageBox.Show( ex3.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(BLTemporaryNotAvailableException ex4)
            {
                MessageBox.Show($"Cannot perform the operation since Simulator is running:)");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        ///// <summary>
        ///// Re-reading call's details
        ///// </summary>
        //private void RefreshCall() //?
        //{
        //    //int id = CurrentCall!.Id;
        //    //CurrentCall = null;
        //    //CurrentCall = s_bl.Call.Read(id);
        //}

        //private void CallObserver() => RefreshCall();

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //    => s_bl.Call.AddObserver(CallObserver);

        //private void Window_Closed(object sender, EventArgs e)
        //    => s_bl.Call.RemoveObserver(CallObserver);

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _preWind.Show();
            this.Close();
        }
    }
}
    

