using System.Windows;
using System.Windows.Input;

namespace MainScreenUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow1 : Window
    {
        public MainWindow1()
        {
            //InitializeComponent();
        }

        private void ButtonLogOut_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void GridCornerButtons_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnClickProfile(object sender, RoutedEventArgs e)
        {
            //PageArea.Content = new UserProfile();
        }
        private void BtnClickExercise(object sender, RoutedEventArgs e)
        {
            //PageArea.Content = new Exercise2();
        }
        private void BtnClickPoints(object sender, RoutedEventArgs e)
        {
            //PageArea.Content = new Points();
        }
    }
}
