using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MainScreenUI
{
    /// <summary>
    /// Interaction logic for Points.xaml
    /// </summary>
    public partial class Points : Page
    {
        public Points()
        {
            InitializeComponent();
        }

        public Points(int countOfExercisesCompleted)
        {
            
        }

        private void GoToExercise(object sender, RoutedEventArgs e)
        {
            Exercise3 exercise = new Exercise3();
            NavigationService.Navigate(exercise);
        }

        private void GoToProfile(object sender, RoutedEventArgs e)
        {
            UserProfile profile = new UserProfile();
            NavigationService.Navigate(profile);
        }
    }
}
