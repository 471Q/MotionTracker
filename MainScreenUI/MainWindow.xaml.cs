using System.Windows;
using System.Windows.Input;

namespace MainScreenUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            
           InitializeComponent();

            PageArea.Content = new login();

           // PageArea.Content = new registration();
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
            PageArea.Content = new UserProfile();
        }
        private void BtnClickExercise(object sender, RoutedEventArgs e)
        {
            PageArea.Content = new Exercise3();
        }
        private void BtnClickPoints(object sender, RoutedEventArgs e)
        {
            PageArea.Content = new Points();
        }
    }
}
