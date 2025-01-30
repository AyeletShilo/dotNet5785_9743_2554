using BO;
using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Threading;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private Window _preWind;

        //constructor
        public CallWindow(Window preWind, int id = 0)
        {

            InitializeComponent();
            CurrentCall = (id != 0) ? s_bl.Call.Read(id)!
                : new BO.Call() { Id = 0, CallAddress = "" };
            _preWind = preWind;
        }

        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

        /// <summary>
        /// Update call window
        /// </summary>
        public void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.Update(CurrentCall!);
                _preWind.Show(); //?
                this.Close();
            }
            catch (BO.BlDoesNotExistException ex1)
            {
                MessageBox.Show($"Call with ID={CurrentCall!.Id} does Not exists", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlXMLFileLoadCreateException ex2)
            {
                MessageBox.Show($"Xml error", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BlIntegrityOfValuesException ex3)
            {
                MessageBox.Show(ex3.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BLTemporaryNotAvailableException)
            {
                MessageBox.Show($"Cannot perform the operation since Simulator is running:)");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "Please try again", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        /// <summary>
        /// Re-reading call's details
        /// </summary>
        private void RefreshCall()
        {
            int id = CurrentCall!.Id;
            CurrentCall = null;
            CurrentCall = s_bl.Call.Read(id);
        }

        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        private void CallObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {

                    RefreshCall();
                });
        } 

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(CallObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(CallObserver);

        //private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //}

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _preWind.Show();
            this.Close();
        }
    }
}
