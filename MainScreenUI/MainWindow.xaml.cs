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
            Main.Content = new UserProfile();
        }
        private void BtnClickExercise(object sender, RoutedEventArgs e)
        {
            Main.Content = new Exercise();
        }
        private void BtnClickPoints(object sender, RoutedEventArgs e)
        {
            Main.Content = new Points();
        }
    }
}
