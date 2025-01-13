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

namespace PL
{
    /// <summary>
    /// Interaction logic for BaseWindow.xaml
    /// </summary>
    public partial class BaseWindow : Window
    {
        private Window _previousWindow;

        public BaseWindow(Window previousWindow = null)
        {
            InitializeComponent();
            _previousWindow = previousWindow;
        }

        protected void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _previousWindow?.Show();
            this.Close();
        }
    }
}
