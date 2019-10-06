using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            IFirebaseConfig ifc = new FirebaseConfig()
            {
                AuthSecret = "5JF2869ie6NEZOnxh2YPqEVnvoa9UdttEdaSeKAG",
                BasePath = "https://motiontracker-dd816.firebaseio.com/"
            };
            FirebaseResponse res = new FireSharp.FirebaseClient(ifc).Get(@"Users/" + Login.userDetail.Username);
            User UserUpdatedPoint = res.ResultAs<User>(); //firebase result

            progressBar.Value = UserUpdatedPoint.Points;
            progressText.Text = UserUpdatedPoint.Points.ToString() + "/100";
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
