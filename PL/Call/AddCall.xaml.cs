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
    /// Interaction logic for AddCall.xaml
    /// </summary>
    public partial class AddCall : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public AddCall()
        {
            InitializeComponent();
            CurrentCall = new BO.Call() { Id = 0};
        }

        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(AddCall), new PropertyMetadata(null));

        public void bthAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.Create(CurrentCall!);
                this.Close();
            }
            catch (BO.BlXMLFileLoadCreateException ex1)
            {
                MessageBox.Show($"Xml Error");
            }
            catch (BlIntegrityOfValuesException ex3)
            {
                MessageBox.Show($"Error in integrity");
            }
        }

        private void RefreshCall()
        {
            int id = CurrentCall!.Id;
            CurrentCall = null;
            CurrentCall = s_bl.Call.Read(id);
        }

        private void CallObserver() => RefreshCall();

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(CallObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(CallObserver);

    }
}
    

