using FireSharp.Response;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;

namespace MainScreenUI
{
    /// <summary>
    /// Interaction logic for Points.xaml
    /// </summary>
    public partial class Points : Page
    {
        FireS fib = new FireS();
        public Points()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            fib.SetIFC();

            FirebaseResponse res = new FireSharp.FirebaseClient(fib.ifc).Get(@"Users/" + Login.userDetail.Username);
            User UserUpdatedPoint = res.ResultAs<User>(); //firebase result

            userName.Text = UserUpdatedPoint.Username;
            progressBar.Value = UserUpdatedPoint.Points;
            progressText.Text = UserUpdatedPoint.Points.ToString() + "/" + UserUpdatedPoint.MaxPoints.ToString();

            //Bronze.Source = new BitmapImage(new Uri(@"/Resources/bronzetrophy.png", UriKind.Relative));
            //Silver.Source = new BitmapImage(new Uri(@"/Resources/silvertrophy.png", UriKind.Relative));
            //Gold.Source = new BitmapImage(new Uri(@"/Resources/goldtrophy.png", UriKind.Relative));
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
