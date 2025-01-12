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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerDataWindow.xaml
    /// </summary>
    public partial class VolunteerDataWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private static int _id;

        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        /// <summary>
        /// DependencyProperty
        /// </summary>
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerDataWindow), new PropertyMetadata(null));
        public VolunteerDataWindow(int id)
        {
            _id = id;
            InitializeComponent();
            CurrentVolunteer = s_bl.Volunteer.Read(_id);
        }

        private void RefreshVolunteer()
        {
            int id = CurrentVolunteer!.Id;
            CurrentVolunteer = null;
            CurrentVolunteer = s_bl.Volunteer.Read(id);
        }

        private void VolunteerObserver() => RefreshVolunteer();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.Update(_id!, CurrentVolunteer!);
            Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
           => s_bl.Volunteer.AddObserver(VolunteerObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(VolunteerObserver);
    }
}
